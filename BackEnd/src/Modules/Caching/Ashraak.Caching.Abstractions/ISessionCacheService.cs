namespace Ashraak.Caching.Abstractions;

/// <summary>
/// High-level cache facade for authentication session metadata.
/// Session data is scoped to tenant + user and stored through the underlying
/// two-tier cache implementation (memory + Redis).
/// </summary>
public interface ISessionCacheService
{
    /// <summary>
    /// Writes or replaces the active session metadata for a tenant user pair.
    /// </summary>
    /// <param name="tenantId">Tenant scope of the session.</param>
    /// <param name="userId">User owning the session.</param>
    /// <param name="session">Session payload to cache.</param>
    /// <param name="expiry">TTL for the cached session record.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    Task SetAsync(
        Guid tenantId,
        Guid userId,
        SessionCacheEntry session,
        TimeSpan expiry,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the active cached session metadata for a tenant user pair, if any.
    /// </summary>
    Task<SessionCacheEntry?> GetAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the active cached session metadata for a tenant user pair.
    /// </summary>
    Task RemoveAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Session metadata persisted in cache for fast lookup and revocation checks.
/// </summary>
/// <param name="TenantId">Tenant scope.</param>
/// <param name="UserId">User identifier.</param>
/// <param name="Roles">Resolved role claims at issuance time.</param>
/// <param name="Permissions">Resolved permission claims at issuance time.</param>
/// <param name="IssuedOnUtc">UTC issuance timestamp.</param>
public sealed record SessionCacheEntry(
    Guid TenantId,
    Guid UserId,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    DateTime IssuedOnUtc);
