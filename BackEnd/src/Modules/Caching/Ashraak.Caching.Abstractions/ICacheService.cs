namespace Ashraak.Caching.Abstractions;

/// <summary>
/// Two-tier cache abstraction used throughout the application.
/// The implementation (<c>RedisCacheService</c>) maintains an L1 in-process
/// <see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/> layer (1-minute TTL)
/// backed by an L2 Redis store (30-minute default TTL).
/// </summary>
/// <remarks>
/// Use <see cref="CacheKeyBuilder"/> to construct structured cache keys that include
/// environment, tenant scope, module, and entity identifiers.
/// </remarks>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by <paramref name="key"/>.
    /// Checks the L1 memory cache first, then Redis.
    /// Returns <see langword="default"/> (<see langword="null"/> for reference types)
    /// when the key does not exist or has expired.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the cached value into.</typeparam>
    /// <param name="key">The fully-qualified cache key (use <see cref="CacheKeyBuilder"/>).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the cached value for <paramref name="key"/> if it exists.
    /// Otherwise invokes <paramref name="factory"/> to produce the value, stores it in both
    /// cache tiers, and returns the fresh value.
    /// Prefer this over separate Get + Set calls to avoid cache stampede on misses.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Async function called on cache miss to produce the value.</param>
    /// <param name="expiry">
    /// The Redis L2 TTL. Defaults to 30 minutes. L1 expiry is capped at 60 seconds.
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores <paramref name="value"/> in both the L1 memory cache and L2 Redis cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to serialise and store.</param>
    /// <param name="expiry">TTL for the Redis entry. Defaults to 30 minutes.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the entry for <paramref name="key"/> from both cache tiers.
    /// Call this whenever the underlying entity is mutated.
    /// </summary>
    /// <param name="key">The cache key to evict.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all keys that start with <paramref name="prefix"/> from both cache tiers.
    /// Used to invalidate an entire tenant's or module's cache segment at once.
    /// </summary>
    /// <param name="prefix">The key prefix to match (e.g. <c>"prod:tenantId:Tenant:"</c>).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="key"/> exists in either the
    /// L1 memory cache or the L2 Redis cache.
    /// </summary>
    /// <param name="key">The cache key to test.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
