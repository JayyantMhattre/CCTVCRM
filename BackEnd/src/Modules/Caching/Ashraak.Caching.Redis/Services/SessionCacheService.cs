using Ashraak.Caching.Abstractions;

namespace Ashraak.Caching.Redis.Services;

/// <summary>
/// Redis-backed implementation of <see cref="ISessionCacheService"/>.
/// Delegates storage to <see cref="ICacheService"/> to benefit from two-tier caching.
/// </summary>
internal sealed class SessionCacheService : ISessionCacheService
{
    private readonly ICacheService _cacheService;

    public SessionCacheService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task SetAsync(
        Guid tenantId,
        Guid userId,
        SessionCacheEntry session,
        TimeSpan expiry,
        CancellationToken cancellationToken = default)
    {
        var key = CacheKeyBuilder.ForSession(tenantId, userId);
        await _cacheService.SetAsync(key, session, expiry, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SessionCacheEntry?> GetAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var key = CacheKeyBuilder.ForSession(tenantId, userId);
        return await _cacheService.GetAsync<SessionCacheEntry>(key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var key = CacheKeyBuilder.ForSession(tenantId, userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
    }
}
