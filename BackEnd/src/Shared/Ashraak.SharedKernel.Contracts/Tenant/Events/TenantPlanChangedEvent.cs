using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Tenant.Events;

/// <summary>
/// Raised when a tenant's subscription plan changes (upgrade or downgrade).
/// Handled by the Audit module to record the plan change, and potentially by the
/// Billing module (future) to trigger proration and invoice generation.
/// </summary>
/// <param name="TenantId">The unique identifier of the affected tenant.</param>
/// <param name="OldPlan">The plan the tenant was on before the change.</param>
/// <param name="NewPlan">The plan the tenant has been moved to.</param>
public sealed record TenantPlanChangedEvent(
    Guid TenantId,
    TenantPlan OldPlan,
    TenantPlan NewPlan) : DomainEvent;
