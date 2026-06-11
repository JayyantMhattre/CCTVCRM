using System.Data;
using System.Linq.Expressions;
using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.BuildingBlocks.Data.Sql;

/// <summary>
/// Provider-agnostic SQL implementation of <see cref="IDataRepository{T}"/> that
/// combines EF Core (write-side + structured reads) and Dapper (raw SQL reads).
///
/// <para>
/// <b>Multi-tenancy</b>: When <typeparamref name="T"/> implements
/// <see cref="ITenantScoped"/>, every query is automatically scoped to the
/// current tenant.  <c>Insert*</c> methods automatically stamp the
/// <c>TenantId</c> from <see cref="ITenantContext"/>.
/// </para>
///
/// <para>
/// <b>Soft-delete</b>: All read methods exclude records where
/// <c>IsDeleted == true</c>.  Hard-delete is available via <c>DeleteAsync</c>.
/// </para>
///
/// <para>
/// <b>Thread safety</b>: Register as <c>Scoped</c> — one instance per HTTP request.
/// </para>
/// </summary>
/// <typeparam name="T">The entity type.  Must inherit <see cref="BaseDataEntity"/>.</typeparam>
public class GenericSqlRepository<T> : IDataRepository<T>
    where T : BaseDataEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly IDbConnectionFactory _connectionFactory;

    // Nullable because background services may run without a tenant context.
    private readonly ITenantContext? _tenantContext;

    // True when the entity carries its own TenantId column.
    private readonly bool _isTenantScoped = typeof(ITenantScoped).IsAssignableFrom(typeof(T));

    /// <summary>
    /// Initialises the repository.
    /// </summary>
    /// <param name="context">The module's EF Core <see cref="DbContext"/>.</param>
    /// <param name="connectionFactory">
    /// Provides open ADO.NET connections for Dapper raw-SQL execution.
    /// </param>
    /// <param name="tenantContext">
    /// Optional: current-request tenant context.  When <see langword="null"/>
    /// (e.g. background job), the TenantId filter is not applied.
    /// </param>
    public GenericSqlRepository(
        DbContext context,
        IDbConnectionFactory connectionFactory,
        ITenantContext? tenantContext = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _tenantContext = tenantContext;
        _dbSet = context.Set<T>();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // READ
    // ═══════════════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Start from the pre-filtered base query (IsDeleted + TenantId).
        return await BaseQuery()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await BaseQuery().ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => await BaseQuery().Where(filter).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<T>> GetByFilterAsync(
        Expression<Func<T, bool>> filter,
        QueryOptions<T> options,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryWithOptions(options).Where(filter);
        return await query.ToListAsync(cancellationToken);
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

        var query = BaseQuery();

        if (filter is not null)
            query = query.Where(filter);

        // Total count before paging (needed for PagedResult.TotalCount).
        var totalCount = await query.LongCountAsync(cancellationToken);

        // Apply optional multi-column sort.
        query = ApplySorting(query, sortDescriptors);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, pageNumber, pageSize, totalCount);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => await BaseQuery().AnyAsync(filter, cancellationToken);

    /// <inheritdoc/>
    public async Task<long> CountAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = BaseQuery();
        return filter is not null
            ? await query.LongCountAsync(filter, cancellationToken)
            : await query.LongCountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="NotSupportedException">Never thrown by the SQL provider.</exception>
    public IQueryable<T> Query(bool tracking = false)
    {
        // Return the pre-filtered base query so callers never accidentally bypass
        // the IsDeleted / TenantId safety net.
        return tracking ? BaseQuery(tracking: true) : BaseQuery();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // WRITE
    // ═══════════════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        StampInsert(entity);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task InsertBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        foreach (var entity in list)
            StampInsert(entity);

        await _dbSet.AddRangeAsync(list, cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateByFilterAsync(
        Expression<Func<T, bool>> filter,
        Action<T> updater,
        CancellationToken cancellationToken = default)
    {
        // Load matching entities with change tracking so EF Core can detect changes.
        var entities = await BaseQuery(tracking: true)
            .Where(filter)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            updater(entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;
        }

        // Changes are persisted on the next SaveChangesAsync call.
        return entities.Count;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteByFilterAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        // EF Core 7+ bulk delete — single SQL DELETE, no load required.
        return await BaseQuery().Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await BaseQuery(tracking: true)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
            return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        return true;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // ADVANCED (Dapper raw SQL)
    // ═══════════════════════════════════════════════════════════════════════

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TResult>> ExecuteRawSqlAsync<TResult>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        // IDbConnection implements IDisposable (not IAsyncDisposable), so use synchronous using.
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        var results = await connection.QueryAsync<TResult>(sql, parameters);
        return results.ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<int> ExecuteStoredProcedureAsync(
        string procedureName,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        // IDbConnection implements IDisposable (not IAsyncDisposable), so use synchronous using.
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteAsync(
            procedureName,
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // PRIVATE HELPERS
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> with the mandatory base filters applied:
    /// <list type="bullet">
    ///   <item><c>IsDeleted == false</c> — excludes soft-deleted records.</item>
    ///   <item><c>TenantId == currentTenantId</c> — when T is <see cref="ITenantScoped"/>
    ///   and a non-empty tenant context is available.</item>
    /// </list>
    /// </summary>
    private IQueryable<T> BaseQuery(bool tracking = false)
    {
        IQueryable<T> query = tracking
            ? _dbSet.AsTracking()
            : _dbSet.AsNoTracking();

        // Exclude soft-deleted records from all default queries.
        query = query.Where(e => !e.IsDeleted);

        // Inject the TenantId filter when this entity carries tenant data
        // and an active (non-empty) tenant context exists.
        if (_isTenantScoped
            && _tenantContext is not null
            && _tenantContext.TenantId != Guid.Empty)
        {
            var tenantId = _tenantContext.TenantId;

            // Cast is safe because _isTenantScoped guarantees T : ITenantScoped.
            query = query.Where(e => ((ITenantScoped)e).TenantId == tenantId);
        }

        return query;
    }

    /// <summary>
    /// Builds a query that also applies any EF Core-specific <see cref="QueryOptions{T}"/>
    /// (includes, split-query, tracking).
    /// </summary>
    private IQueryable<T> BuildQueryWithOptions(QueryOptions<T> options)
    {
        var query = BaseQuery(options.TrackChanges);

        // Apply eager-load includes.
        foreach (var include in options.Includes)
            query = query.Include(include);

        // Enable split-query when multiple collection navigations are included
        // to avoid the cartesian-explosion problem.
        if (options.SplitQuery && options.Includes.Count > 0)
            query = query.AsSplitQuery();

        return query;
    }

    /// <summary>
    /// Applies a list of <see cref="SortDescriptor{T}"/> to a query.
    /// When no descriptors are provided, the query is returned unchanged.
    /// </summary>
    private static IQueryable<T> ApplySorting(
        IQueryable<T> query,
        IEnumerable<SortDescriptor<T>>? descriptors)
    {
        var list = descriptors?.ToList();
        if (list is null || list.Count == 0) return query;

        // First sort clause uses OrderBy / OrderByDescending.
        IOrderedQueryable<T>? ordered = list[0].Descending
            ? query.OrderByDescending(list[0].KeySelector)
            : query.OrderBy(list[0].KeySelector);

        // Subsequent clauses use ThenBy / ThenByDescending.
        for (var i = 1; i < list.Count; i++)
        {
            ordered = list[i].Descending
                ? ordered.ThenByDescending(list[i].KeySelector)
                : ordered.ThenBy(list[i].KeySelector);
        }

        return ordered;
    }

    /// <summary>
    /// Stamps audit fields on insert: sets <c>CreatedAtUtc</c> and (when tenant-scoped)
    /// the <c>TenantId</c> from the active <see cref="ITenantContext"/>.
    /// </summary>
    private void StampInsert(T entity)
    {
        entity.CreatedAtUtc = DateTime.UtcNow;

        // Auto-assign TenantId from the active tenant context when not already set.
        if (_isTenantScoped
            && _tenantContext is not null
            && _tenantContext.TenantId != Guid.Empty
            && entity is ITenantScoped scopedEntity
            && scopedEntity.TenantId == Guid.Empty)
        {
            // TenantScopedEntity.TenantId has a public setter, so this is safe.
            ((TenantScopedEntity)(object)entity).TenantId = _tenantContext.TenantId;
        }
    }
}
