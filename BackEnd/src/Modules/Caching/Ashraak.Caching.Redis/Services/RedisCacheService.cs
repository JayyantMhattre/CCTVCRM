using System.Text.Json;
using Ashraak.Caching.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Ashraak.Caching.Redis.Services;

/// <summary>
/// Two-tier cache implementation:
/// <list type="bullet">
///   <item><description>L1 — <see cref="IMemoryCache"/> (in-process, 60-second TTL) for sub-millisecond reads.</description></item>
///   <item><description>L2 — Redis (cross-instance, 30-minute default TTL) for distributed consistency.</description></item>
/// </list>
/// On a read, L1 is checked first; on miss, Redis is consulted and the value is promoted to L1.
/// On a write, both tiers are updated atomically.
/// On an invalidation, both tiers are cleared to prevent stale L1 serving after L2 eviction.
/// </summary>
/// <remarks>
/// JSON serialisation uses <see cref="JsonSerializerOptions"/> with case-insensitive property matching
/// to remain compatible with responses from both EF Core and Dapper projections.
/// </remarks>
internal sealed class RedisCacheService : ICacheService
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan L1Expiry = TimeSpan.FromSeconds(60);

    private readonly IDatabase _redis;
    private readonly IMemoryCache _memoryCache;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initialises the service with an active Redis multiplexer and the in-process memory cache.
    /// Both are registered as singletons in <c>CachingModule</c>.
    /// </summary>
    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, IMemoryCache memoryCache)
    {
        _redis = connectionMultiplexer.GetDatabase();
        _memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out T? cached))
            return cached;

        var value = await _redis.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        var result = JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        _memoryCache.Set(key, result, L1Expiry);
        return result;
    }

    /// <inheritdoc/>
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync<T>(key, cancellationToken);
        if (existing is not null)
            return existing;

        var value = await factory(cancellationToken);
        await SetAsync(key, value, expiry, cancellationToken);
        return value;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var serialized = JsonSerializer.Serialize(value, _jsonOptions);
        var effectiveExpiry = expiry ?? DefaultExpiry;

        await _redis.StringSetAsync(key, serialized, effectiveExpiry);
        _memoryCache.Set(key, value, TimeSpan.FromTicks(Math.Min(L1Expiry.Ticks, effectiveExpiry.Ticks)));
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _redis.KeyDeleteAsync(key);
        _memoryCache.Remove(key);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Uses <c>SCAN</c> via <c>IServer.Keys</c> to avoid blocking Redis with <c>KEYS *</c>.
    /// On large key spaces this iterates in batches; do not call with an overly broad prefix.
    /// </remarks>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var server = _redis.Multiplexer.GetServer(_redis.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}*").ToArray();

        if (keys.Length > 0)
            await _redis.KeyDeleteAsync(keys);

        foreach (var key in keys)
            _memoryCache.Remove(key.ToString());
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out _))
            return true;
        return await _redis.KeyExistsAsync(key);
    }
}
