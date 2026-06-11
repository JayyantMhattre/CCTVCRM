using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using MongoDB.Driver;

namespace Ashraak.BuildingBlocks.Data.Mongo;

/// <summary>
/// Helper service for creating and managing MongoDB indexes on a collection.
///
/// <para>
/// Call from the module's startup / hosted service to ensure required indexes
/// exist before the first query.  MongoDB creates the index only when it doesn't
/// already exist, so calling these methods on every startup is safe (idempotent).
/// </para>
///
/// <example>
/// <code>
/// // In module DI extension (AddAuditModule):
/// services.AddSingleton&lt;IHostedService&gt;(sp =>
///     new MongoIndexBootstrapService(
///         sp.GetRequiredService&lt;IMongoDatabase&gt;(),
///         async indexService =>
///         {
///             await indexService.CreateAscendingIndexAsync&lt;AuditLog&gt;(x => x.TenantId);
///             await indexService.CreateAscendingIndexAsync&lt;AuditLog&gt;(x => x.CreatedAtUtc);
///             await indexService.CreateTextIndexAsync&lt;AuditLog&gt;(x => x.Action);
///         }));
/// </code>
/// </example>
/// </summary>
public sealed class MongoIndexService
{
    private readonly IMongoDatabase _database;

    public MongoIndexService(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    /// <summary>
    /// Creates an ascending single-field index on <paramref name="field"/>
    /// for collection <typeparamref name="T"/>.
    /// </summary>
    public async Task CreateAscendingIndexAsync<T>(
        System.Linq.Expressions.Expression<Func<T, object?>> field,
        bool unique = false,
        string? collectionName = null,
        CancellationToken cancellationToken = default)
        where T : BaseDataEntity
    {
        var collection = GetCollection<T>(collectionName);
        var indexKey = Builders<T>.IndexKeys.Ascending(field);
        var options = new CreateIndexOptions { Unique = unique, Background = true };
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<T>(indexKey, options),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a descending single-field index on <paramref name="field"/>
    /// for collection <typeparamref name="T"/>.
    /// </summary>
    public async Task CreateDescendingIndexAsync<T>(
        System.Linq.Expressions.Expression<Func<T, object?>> field,
        bool unique = false,
        string? collectionName = null,
        CancellationToken cancellationToken = default)
        where T : BaseDataEntity
    {
        var collection = GetCollection<T>(collectionName);
        var indexKey = Builders<T>.IndexKeys.Descending(field);
        var options = new CreateIndexOptions { Unique = unique, Background = true };
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<T>(indexKey, options),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a compound index on multiple fields for collection <typeparamref name="T"/>.
    /// Compound indexes are highly efficient for queries that filter on multiple
    /// fields simultaneously (e.g. <c>TenantId + IsDeleted + CreatedAtUtc</c>).
    /// </summary>
    public async Task CreateCompoundIndexAsync<T>(
        IEnumerable<(System.Linq.Expressions.Expression<Func<T, object?>> Field, bool Descending)> fields,
        bool unique = false,
        string? collectionName = null,
        CancellationToken cancellationToken = default)
        where T : BaseDataEntity
    {
        var collection = GetCollection<T>(collectionName);
        var keyBuilder = Builders<T>.IndexKeys;

        // Combine all field definitions into a single compound key.
        var keys = fields.Select(f =>
            f.Descending ? keyBuilder.Descending(f.Field) : keyBuilder.Ascending(f.Field));

        var compoundKey = keyBuilder.Combine(keys);
        var options = new CreateIndexOptions { Unique = unique, Background = true };

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<T>(compoundKey, options),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a full-text search index on <paramref name="field"/> for collection
    /// <typeparamref name="T"/>.  Only one text index is allowed per collection.
    /// </summary>
    public async Task CreateTextIndexAsync<T>(
        System.Linq.Expressions.Expression<Func<T, object?>> field,
        string? collectionName = null,
        CancellationToken cancellationToken = default)
        where T : BaseDataEntity
    {
        var collection = GetCollection<T>(collectionName);
        var indexKey = Builders<T>.IndexKeys.Text(field);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<T>(indexKey),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a TTL (Time-To-Live) index that automatically deletes documents
    /// after the specified duration.  Useful for session, token, or cache collections.
    /// </summary>
    /// <param name="field">The <see cref="DateTime"/> field to base expiration on.</param>
    /// <param name="expireAfter">How long documents live before automatic deletion.</param>
    public async Task CreateTtlIndexAsync<T>(
        System.Linq.Expressions.Expression<Func<T, object?>> field,
        TimeSpan expireAfter,
        string? collectionName = null,
        CancellationToken cancellationToken = default)
        where T : BaseDataEntity
    {
        var collection = GetCollection<T>(collectionName);
        var indexKey = Builders<T>.IndexKeys.Ascending(field);
        var options = new CreateIndexOptions { ExpireAfter = expireAfter, Background = true };
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<T>(indexKey, options),
            cancellationToken: cancellationToken);
    }

    /// <summary>Lists all indexes for the collection mapped to <typeparamref name="T"/>.</summary>
    public async Task<IReadOnlyList<string>> ListIndexNamesAsync<T>(
        string? collectionName = null,
        CancellationToken cancellationToken = default)
        where T : BaseDataEntity
    {
        var collection = GetCollection<T>(collectionName);
        var cursor = await collection.Indexes.ListAsync(cancellationToken);
        var docs = await cursor.ToListAsync(cancellationToken);
        return docs.Select(d => d["name"].AsString).ToList().AsReadOnly();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private IMongoCollection<T> GetCollection<T>(string? name)
        where T : BaseDataEntity
    {
        var collectionName = name ?? ToCollectionName(typeof(T).Name);
        return _database.GetCollection<T>(collectionName);
    }

    private static string ToCollectionName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return "documents";
        var camel = char.ToLowerInvariant(typeName[0]) + typeName[1..];
        return camel.EndsWith('s') ? camel : camel + "s";
    }
}
