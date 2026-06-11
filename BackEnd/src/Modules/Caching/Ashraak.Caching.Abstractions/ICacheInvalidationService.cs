namespace Ashraak.Caching.Abstractions;

/// <summary>
/// Centralised invalidation strategy entry point for common cache segments.
/// Keeps invalidation logic consistent across modules and avoids ad-hoc key handling.
/// </summary>
public interface ICacheInvalidationService
{
    /// <summary>
    /// Invalidates cached permission snapshot for a single user in a tenant.
    /// Call this after role/permission assignment changes.
    /// </summary>
    Task InvalidatePermissionsAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cached session metadata for a single user in a tenant.
    /// Call this on logout, token revocation, or user deactivation.
    /// </summary>
    Task InvalidateSessionAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates tenant configuration caches (entity + feature-flag scopes).
    /// Call this when tenant plan/settings/feature flags change.
    /// </summary>
    Task InvalidateTenantConfigAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all cache entries for a tenant module prefix.
    /// Useful for coarse-grained fallback invalidation.
    /// </summary>
    Task InvalidateModulePrefixAsync(Guid tenantId, string module, CancellationToken cancellationToken = default);
}
