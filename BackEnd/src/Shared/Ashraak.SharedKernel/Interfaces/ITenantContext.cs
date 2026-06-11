namespace Ashraak.SharedKernel.Interfaces;

/// <summary>
/// Provides the multi-tenant context for the current HTTP request.
/// Implemented by <c>TenantContext</c> in the Host API and resolved from the
/// route, header (<c>X-Tenant-Id</c>), or JWT claim.
/// </summary>
/// <remarks>
/// Used by module infrastructure layers (EF Core query filters, repositories) to
/// scope data access to the active tenant without the caller needing to pass a
/// tenant identifier explicitly.
/// PostgreSQL Row-Level Security policies rely on the same identifier being set
/// as a session variable via <c>SET app.tenant_id = '...'</c>.
/// </remarks>
public interface ITenantContext
{
    /// <summary>Gets the unique identifier of the current tenant.</summary>
    Guid TenantId { get; }

    /// <summary>
    /// Gets the human-readable URL slug for the current tenant
    /// (e.g. <c>"acme"</c> for <c>https://app.acme.saas.com</c>).
    /// </summary>
    string TenantSlug { get; }

    /// <summary>
    /// Gets the subscription plan of the current tenant
    /// (e.g. <c>"Free"</c>, <c>"Pro"</c>, <c>"Enterprise"</c>).
    /// Used for feature flagging and plan-based resource limits.
    /// </summary>
    string Plan { get; }

    /// <summary>
    /// Gets a value indicating whether the tenant account is active.
    /// Suspended tenants receive <c>403 Forbidden</c> at the gateway level.
    /// </summary>
    bool IsActive { get; }
}
