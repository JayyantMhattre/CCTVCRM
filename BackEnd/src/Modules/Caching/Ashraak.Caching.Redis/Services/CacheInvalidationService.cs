using Ashraak.Caching.Abstractions;

namespace Ashraak.Caching.Redis.Services;

/// <summary>
/// Default invalidation strategy implementation for common cache segments.
/// Encapsulates key construction and invalidation ordering in one place.
/// </summary>
internal sealed class CacheInvalidationService : ICacheInvalidationService
{
    private readonly ICacheService _cacheService;

    public CacheInvalidationService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task InvalidatePermissionsAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var key = CacheKeyBuilder.ForPermissions(tenantId, userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task InvalidateSessionAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var key = CacheKeyBuilder.ForSession(tenantId, userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task InvalidateTenantConfigAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenantConfigKey = CacheKeyBuilder.ForEntity(tenantId, "tenant", "config", tenantId);
        var featureFlagsKey = CacheKeyBuilder.ForFeatureFlags(tenantId);

        // Invalidate specific keys first, then module prefix as a safe fallback.
        await _cacheService.RemoveAsync(tenantConfigKey, cancellationToken);
        await _cacheService.RemoveAsync(featureFlagsKey, cancellationToken);
        await _cacheService.RemoveByPrefixAsync(
            CacheKeyBuilder.ForTenantPrefix(tenantId, "tenant"),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task InvalidateModulePrefixAsync(
        Guid tenantId,
        string module,
        CancellationToken cancellationToken = default)
    {
        await _cacheService.RemoveByPrefixAsync(
            CacheKeyBuilder.ForTenantPrefix(tenantId, module),
            cancellationToken);
    }
}
