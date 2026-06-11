namespace Ashraak.SharedKernel.Contracts.Tenant.Dtos;

/// <summary>
/// Subscription tiers available to a tenant.
/// Determines feature access, seat limits, and SLA guarantees.
/// </summary>
public enum TenantPlan
{
    /// <summary>Free tier — limited seats and features, community support only.</summary>
    Free,

    /// <summary>Professional tier — higher limits, standard SLA, priority support.</summary>
    Pro,

    /// <summary>Enterprise tier — unlimited seats, custom SLA, SSO, dedicated infrastructure.</summary>
    Enterprise
}

/// <summary>
/// Lifecycle status of a tenant account.
/// </summary>
public enum TenantStatus
{
    /// <summary>The tenant is operational; users can log in and use the platform.</summary>
    Active,

    /// <summary>The tenant has been suspended (e.g. non-payment). Users receive 403 responses.</summary>
    Suspended,

    /// <summary>The tenant has been soft-deleted. Data is retained for the grace period then purged.</summary>
    Deleted
}

/// <summary>
/// Read-only projection of a <c>Tenant</c> aggregate exposed across module boundaries.
/// Returned by <c>ITenantService</c> and published as part of integration events.
/// </summary>
/// <param name="TenantId">Unique identifier of the tenant.</param>
/// <param name="Name">Human-readable display name (e.g. <c>"Acme Corporation"</c>).</param>
/// <param name="Slug">URL-safe identifier used in subdomains and API paths (e.g. <c>"acme"</c>).</param>
/// <param name="Plan">Current subscription plan.</param>
/// <param name="Status">Current lifecycle status.</param>
/// <param name="CustomDomain">Optional CNAME the tenant has mapped to the platform.</param>
/// <param name="CreatedOnUtc">UTC timestamp of tenant provisioning.</param>
public sealed record TenantDto(
    Guid TenantId,
    string Name,
    string Slug,
    TenantPlan Plan,
    TenantStatus Status,
    string? CustomDomain,
    DateTime CreatedOnUtc);
