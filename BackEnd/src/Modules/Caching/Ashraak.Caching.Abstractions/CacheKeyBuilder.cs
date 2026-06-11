namespace Ashraak.Caching.Abstractions;

/// <summary>
/// Centralised factory for constructing deterministic, structured Redis cache keys.
/// All keys follow the pattern <c>{env}:{tenantId}:{module}:{resource}:{id}</c>
/// to ensure multi-tenant isolation and enable prefix-based invalidation.
/// </summary>
/// <remarks>
/// Call <see cref="SetEnvironment"/> during application startup with the value of
/// <c>ASPNETCORE_ENVIRONMENT</c> (lowercased) so that <c>dev</c>, <c>staging</c>,
/// and <c>production</c> keys never collide when sharing a Redis instance.
/// </remarks>
public static class CacheKeyBuilder
{
    private static string _environment = "dev";

    /// <summary>
    /// Sets the environment prefix used by all key builders.
    /// Called once from <c>CachingModule.AddCachingModule</c> on startup.
    /// </summary>
    /// <param name="environment">Lowercase environment name (e.g. <c>"prod"</c>, <c>"staging"</c>).</param>
    public static void SetEnvironment(string environment) => _environment = environment;

    /// <summary>
    /// Builds a key for a single entity, scoped to a tenant.
    /// Example: <c>"prod:acme-guid:Tenant:Tenant:abc123"</c>
    /// </summary>
    /// <param name="tenantId">The tenant the entity belongs to.</param>
    /// <param name="module">Module name (e.g. <c>"Tenant"</c>, <c>"Users"</c>).</param>
    /// <param name="entity">Entity type name (e.g. <c>"Tenant"</c>, <c>"UserProfile"</c>).</param>
    /// <param name="id">String representation of the entity identifier.</param>
    public static string ForEntity(Guid tenantId, string module, string entity, string id) =>
        $"{_environment}:{tenantId}:{module}:{entity}:{id}";

    /// <summary>
    /// Overload of <see cref="ForEntity(Guid,string,string,string)"/> that accepts a <see cref="Guid"/> id.
    /// Uses the compact <c>N</c> format (no hyphens) to save key space.
    /// </summary>
    public static string ForEntity(Guid tenantId, string module, string entity, Guid id) =>
        ForEntity(tenantId, module, entity, id.ToString("N"));

    /// <summary>
    /// Builds a prefix that covers all keys for a given tenant and module.
    /// Pass to <see cref="ICacheService.RemoveByPrefixAsync"/> to invalidate
    /// all cached entries for that module when data changes.
    /// </summary>
    /// <param name="tenantId">The tenant whose module cache should be invalidated.</param>
    /// <param name="module">Module name.</param>
    public static string ForTenantPrefix(Guid tenantId, string module) =>
        $"{_environment}:{tenantId}:{module}:";

    /// <summary>
    /// Builds the key for storing a user's session data (e.g. refresh token metadata).
    /// </summary>
    /// <param name="tenantId">The tenant context.</param>
    /// <param name="userId">The user identifier.</param>
    public static string ForSession(Guid tenantId, Guid userId) =>
        $"{_environment}:{tenantId}:session:{userId:N}";

    /// <summary>
    /// Builds the key for caching a user's resolved permissions list.
    /// Invalidated when a role or permission is modified via <see cref="ICacheService.RemoveAsync"/>.
    /// </summary>
    /// <param name="tenantId">The tenant context.</param>
    /// <param name="userId">The user whose permissions are cached.</param>
    public static string ForPermissions(Guid tenantId, Guid userId) =>
        $"{_environment}:{tenantId}:auth:perms:{userId:N}";

    /// <summary>
    /// Builds the key for caching a tenant's feature flags map.
    /// Invalidated when any feature flag is toggled for the tenant.
    /// </summary>
    /// <param name="tenantId">The tenant whose feature flags are cached.</param>
    public static string ForFeatureFlags(Guid tenantId) =>
        $"{_environment}:{tenantId}:tenant:features";

    /// <summary>
    /// Builds the key used by the Redis rate-limiting middleware for a given endpoint.
    /// Does not include the environment prefix so that rate limit counters work
    /// consistently across instances.
    /// </summary>
    /// <param name="tenantId">The tenant being rate-limited.</param>
    /// <param name="endpoint">The route group or endpoint identifier.</param>
    public static string ForRateLimit(Guid tenantId, string endpoint) =>
        $"ratelimit:{tenantId:N}:{endpoint}";

    /// <summary>
    /// Builds a rate-limit key scoped by tenant, route group, and client IP.
    /// </summary>
    public static string ForRateLimit(Guid tenantId, string routeGroup, string clientIp) =>
        $"ratelimit:{tenantId:N}:{routeGroup}:{clientIp}";

    /// <summary>
    /// Builds the Redis key for a named distributed lock scoped to a tenant.
    /// </summary>
    /// <param name="resource">The operation being locked (e.g. <c>"provision-tenant"</c>).</param>
    /// <param name="tenantId">The tenant context the lock is scoped to.</param>
    public static string ForDistributedLock(string resource, Guid tenantId) =>
        $"lock:{resource}:{tenantId:N}";
}
