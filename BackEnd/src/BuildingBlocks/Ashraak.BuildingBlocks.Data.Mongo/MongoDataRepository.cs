using System.Linq.Expressions;
using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Ashraak.BuildingBlocks.Data.Mongo;

/// <summary>
/// MongoDB implementation of <see cref="IDataRepository{T}"/>.
///
/// <para>
/// Uses the official <c>MongoDB.Driver</c> with its strongly-typed
/// <see cref="FilterDefinitionBuilder{TDocument}"/> and
/// <see cref="UpdateDefinitionBuilder{TDocument}"/> APIs.
/// </para>
///
/// <para>
/// <b>Collection naming</b>: By default the collection name is the
/// lower-camel-case plural of the entity type name, e.g. <c>Invoice → invoices</c>.
/// Override by registering a custom collection name in the module's DI setup.
/// </para>
///
/// <para>
/// <b>Multi-tenancy</b>: All operations automatically scope to the active tenant
/// when <typeparamref name="T"/> implements <see cref="ITenantScoped"/>.
/// </para>
///
/// <para>
/// <b>Soft-delete</b>: All reads exclude <c>IsDeleted == true</c> documents.
/// Hard delete is available via <see cref="DeleteAsync"/> / <see cref="DeleteByFilterAsync"/>.
/// </para>
///
/// <para>
/// <b>Transactions</b>: Write operations use the optional
/// <see cref="IClientSessionHandle"/> managed by <see cref="MongoDataUnitOfWork"/>.
/// </para>
///
/// <para>
/// <b>Advanced (raw SQL)</b>: MongoDB does not support SQL; <see cref="ExecuteRawSqlAsync{TResult}"/>
/// and <see cref="ExecuteStoredProcedureAsync"/> always throw <see cref="NotSupportedException"/>.
/// </para>
/// </summary>
public class MongoDataRepository<T> : IDataRepository<T>
    where T : BaseDataEntity
{
    private readonly IMongoCollection<T> _collection;
    private readonly ITenantContext? _tenantContext;
    private readonly MongoDataUnitOfWork _uow;

    // Cache the check — reflection is cheap but this avoids the call per operation.
    private readonly bool _isTenantScoped = typeof(ITenantScoped).IsAssignableFrom(typeof(T));

    // Shorthand filter builder used throughout this class.
    private static readonly FilterDefinitionBuilder<T> Filters = Builders<T>.Filter;

    /// <param name="database">The MongoDB database resolved from DI.</param>
    /// <param name="uow">Unit of work that manages the optional client session.</param>
    /// <param name="collectionName">
    /// Explicit collection name.  When <see langword="null"/> defaults to
    /// the lower-camel-case plural of <typeparamref name="T"/>.
    /// </param>
    /// <param name="tenantContext">Optional current-request tenant context.</param>
    public MongoDataRepository(
        IMongoDatabase database,
        MongoDataUnitOfWork uow,
        string? collectionName = null,
        ITenantContext? tenantContext = null)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _tenantContext = tenantContext;

        var name = collectionName ?? ToCollectionName(typeof(T).Name);
        _collection = database.GetCollection<T>(name);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // READ
    // ═══════════════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = CombineWithBase(Filters.Eq(e => e.Id, id));
        return await _collection
            .Find(_uow.Session, filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var results = await _collection
            .Find(_uow.Session, BaseFilter())
            .ToListAsync(cancellationToken);
        return results.AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var combined = CombineWithBase(Filters.Where(filter));
        var results = await _collection
            .Find(_uow.Session, combined)
            .ToListAsync(cancellationToken);
        return results.AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        QueryOptions<T> options,
        CancellationToken cancellationToken = default)
    {
        // QueryOptions<T>.Includes and SplitQuery are EF Core-only; MongoDB ignores them.
        return await GetByFilterAsync(filter, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        IEnumerable<SortDescriptor<T>>? sortDescriptors = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Must be >= 1.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be >= 1.");

        var combinedFilter = filter is not null
            ? CombineWithBase(Filters.Where(filter))
            : BaseFilter();

        // Count before paging.
        var totalCount = await _collection
            .CountDocumentsAsync(_uow.Session, combinedFilter, cancellationToken: cancellationToken);

        // Build sort definition.
        var sortDef = BuildSort(sortDescriptors);

        var cursor = _collection.Find(_uow.Session, combinedFilter);
        if (sortDef is not null) cursor = cursor.Sort(sortDef);

        var items = await cursor
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items.AsReadOnly(), pageNumber, pageSize, totalCount);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var combined = CombineWithBase(Filters.Where(filter));
        var count = await _collection
            .CountDocumentsAsync(_uow.Session, combined, cancellationToken: cancellationToken);
        return count > 0;
    }

    /// <inheritdoc/>
    public async Task<long> CountAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        var combinedFilter = filter is not null
            ? CombineWithBase(Filters.Where(filter))
            : BaseFilter();

        return await _collection
            .CountDocumentsAsync(_uow.Session, combinedFilter, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">
    /// Always thrown — MongoDB does not support <see cref="IQueryable{T}"/>.
    /// Use <see cref="GetByFilterAsync"/> or <see cref="GetPagedAsync"/> instead.
    /// </exception>
    public IQueryable<T> Query(bool tracking = false) =>
        throw new NotSupportedException(
            "IQueryable is not supported by the MongoDB provider.  " +
            "Use GetByFilterAsync / GetPagedAsync instead.");

    // ═══════════════════════════════════════════════════════════════════════
    // WRITE
    // ═══════════════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        StampInsert(entity);
        await _collection.InsertOneAsync(_uow.Session, entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task InsertBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        foreach (var entity in list)
            StampInsert(entity);

        await _collection.InsertManyAsync(_uow.Session, list, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        var filter = Filters.Eq(e => e.Id, entity.Id);
        await _collection.ReplaceOneAsync(_uow.Session, filter, entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateByFilterAsync(
        Expression<Func<T, bool>> filter,
        Action<T> updater,
        CancellationToken cancellationToken = default)
    {
        // Load matching documents, apply the in-memory updater, then replace each.
        // For high-throughput bulk updates use MongoDB aggregation pipelines directly.
        var combined = CombineWithBase(Filters.Where(filter));
        var docs = await _collection
            .Find(_uow.Session, combined)
            .ToListAsync(cancellationToken);

        foreach (var doc in docs)
        {
            updater(doc);
            doc.UpdatedAtUtc = DateTime.UtcNow;
            var idFilter = Filters.Eq(e => e.Id, doc.Id);
            await _collection.ReplaceOneAsync(_uow.Session, idFilter, doc, cancellationToken: cancellationToken);
        }

        return docs.Count;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filter = Filters.Eq(e => e.Id, entity.Id);
        await _collection.DeleteOneAsync(_uow.Session, filter, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var combined = CombineWithBase(Filters.Where(filter));
        var result = await _collection
            .DeleteManyAsync(_uow.Session, combined, cancellationToken: cancellationToken);
        return (int)result.DeletedCount;
    }

    /// <inheritdoc/>
    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = CombineWithBase(Filters.Eq(e => e.Id, id));
        var update = Builders<T>.Update
            .Set(e => e.IsDeleted, true)
            .Set(e => e.DeletedAtUtc, DateTime.UtcNow)
            .Set(e => e.UpdatedAtUtc, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(
            _uow.Session, filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // ADVANCED — not supported for MongoDB
    // ═══════════════════════════════════════════════════════════════════════

    /// <exception cref="NotSupportedException">Always thrown — MongoDB has no SQL.</exception>
    public Task<IReadOnlyList<TResult>> ExecuteRawSqlAsync<TResult>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "ExecuteRawSqlAsync is not supported by the MongoDB provider.  " +
            "Use MongoDB aggregation pipelines via GetByFilterAsync or IMongoCollection directly.");

    /// <exception cref="NotSupportedException">Always thrown — MongoDB has no stored procedures.</exception>
    public Task<int> ExecuteStoredProcedureAsync(
        string procedureName,
        object? parameters = null,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "ExecuteStoredProcedureAsync is not supported by the MongoDB provider.");

    // ═══════════════════════════════════════════════════════════════════════
    // PRIVATE HELPERS
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Builds the mandatory base filter:
    /// <c>IsDeleted == false</c> + <c>TenantId == currentTenant</c> (when applicable).
    /// </summary>
    private FilterDefinition<T> BaseFilter()
    {
        // Exclude soft-deleted documents.
        var baseFilter = Filters.Eq(e => e.IsDeleted, false);

        if (_isTenantScoped
            && _tenantContext is not null
            && _tenantContext.TenantId != Guid.Empty)
        {
            var tenantId = _tenantContext.TenantId;
            // Access TenantId through the ITenantScoped interface using BsonElement name.
            var tenantFilter = Filters.Eq("TenantId", tenantId.ToString());
            return Filters.And(baseFilter, tenantFilter);
        }

        return baseFilter;
    }

    /// <summary>Combines an additional filter with the mandatory base filters.</summary>
    private FilterDefinition<T> CombineWithBase(FilterDefinition<T> additional)
        => Filters.And(BaseFilter(), additional);

    /// <summary>Stamps audit and tenant fields on a new document before insertion.</summary>
    private void StampInsert(T entity)
    {
        entity.CreatedAtUtc = DateTime.UtcNow;

        if (_isTenantScoped
            && _tenantContext is not null
            && _tenantContext.TenantId != Guid.Empty
            && entity is TenantScopedEntity tenantEntity
            && tenantEntity.TenantId == Guid.Empty)
        {
            tenantEntity.TenantId = _tenantContext.TenantId;
        }
    }

    /// <summary>
    /// Converts a Pascal-case type name to a lower-camel-case plural collection name.
    /// Example: <c>Invoice → invoices</c>, <c>AuditLog → auditLogs</c>.
    /// </summary>
    private static string ToCollectionName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return "documents";
        var camel = char.ToLowerInvariant(typeName[0]) + typeName[1..];
        return camel.EndsWith('s') ? camel : camel + "s";
    }

    /// <summary>
    /// Builds a MongoDB <see cref="SortDefinition{TDocument}"/> from a list of
    /// <see cref="SortDescriptor{T}"/> objects.
    /// </summary>
    private static SortDefinition<T>? BuildSort(IEnumerable<SortDescriptor<T>>? descriptors)
    {
        var list = descriptors?.ToList();
        if (list is null || list.Count == 0) return null;

        var sortBuilder = Builders<T>.Sort;
        var defs = list.Select(d =>
            d.Descending
                ? sortBuilder.Descending(d.KeySelector)
                : sortBuilder.Ascending(d.KeySelector));

        return sortBuilder.Combine(defs);
    }
}
