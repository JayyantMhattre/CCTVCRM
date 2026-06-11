using Ashraak.SharedKernel.Contracts.Tenant.Dtos;

namespace Ashraak.SharedKernel.Contracts.Tenant.Interfaces;

/// <summary>
/// Cross-module facade for reading tenant data from the Tenant module.
/// Implemented by <c>TenantService</c> in <c>Ashraak.Tenant.Infrastructure</c>.
/// </summary>
/// <remarks>
/// Other modules (Users, Audit, Auth) depend on this interface instead of accessing
/// the Tenant database directly. This preserves module isolation while allowing
/// synchronous in-process queries via the shared interface.
/// For high-throughput reads, implementations are expected to consult the Redis
/// cache (<c>ICacheService</c>) before hitting the database.
/// </remarks>
public interface ITenantService
{
    /// <summary>
    /// Returns a <see cref="TenantDto"/> for the given <paramref name="tenantId"/>,
    /// or <see langword="null"/> if no tenant with that identifier exists.
    /// </summary>
    /// <param name="tenantId">The tenant to look up.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<TenantDto?> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when the tenant exists and its status is <c>Active</c>.
    /// Used by middleware to gate all requests before they reach module handlers.
    /// </summary>
    /// <param name="tenantId">The tenant to check.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> IsActiveAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates a named feature flag for a tenant.
    /// Feature flags are stored in MongoDB and cached in Redis for low latency.
    /// </summary>
    /// <param name="tenantId">The tenant whose flags are evaluated.</param>
    /// <param name="flagName">The feature flag identifier (e.g. <c>"EnableAdvancedReporting"</c>).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    /// <returns><see langword="true"/> if the flag is enabled for this tenant; <see langword="false"/> otherwise.</returns>
    Task<bool> GetFeatureFlagAsync(Guid tenantId, string flagName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the maximum number of user seats allowed by the tenant's current plan.
    /// Enforced by the Users module during invitation and user-creation flows.
    /// </summary>
    /// <param name="tenantId">The tenant to query.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<int> GetSeatLimitAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns workspace settings for the tenant (MFA policy, session timeout, locale).
    /// </summary>
    Task<TenantSettingsDto?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
