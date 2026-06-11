namespace Ashraak.Tenant.Domain.Enums;

/// <summary>
/// Subscription tiers available to a tenant within the Tenant domain.
/// Integer values match <c>SharedKernel.Contracts.Tenant.Dtos.TenantPlan</c> to allow
/// safe explicit casting between the domain and contract enums without a mapping layer.
/// </summary>
public enum TenantPlan
{
    /// <summary>Free tier — 5 seats, 1 GB storage, community support.</summary>
    Free,

    /// <summary>Professional tier — 50 seats, 50 GB storage, standard SLA.</summary>
    Pro,

    /// <summary>Enterprise tier — unlimited seats, 1 TB storage, custom SLA and SSO.</summary>
    Enterprise
}

/// <summary>
/// Lifecycle status of a tenant account within the Tenant domain.
/// Integer values match <c>SharedKernel.Contracts.Tenant.Dtos.TenantStatus</c>.
/// </summary>
public enum TenantStatus
{
    /// <summary>The tenant is operational.</summary>
    Active,

    /// <summary>The tenant has been suspended; users receive 403 responses.</summary>
    Suspended,

    /// <summary>The tenant has been soft-deleted; pending data purge.</summary>
    Deleted
}
