using Ashraak.SharedKernel.Domain.Events;
using Ashraak.Tenant.Domain.Enums;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant.Events;

/// <summary>
/// Raised by <see cref="Tenant.ChangePlan"/> when a tenant's subscription plan is upgraded or downgraded.
/// Handled by the Audit module and, in future, by a Billing module for proration.
/// </summary>
/// <param name="TenantId">The identifier of the affected tenant.</param>
/// <param name="OldPlan">The plan before the change.</param>
/// <param name="NewPlan">The plan after the change.</param>
public sealed record TenantPlanChangedDomainEvent(
    Guid TenantId,
    TenantPlan OldPlan,
    TenantPlan NewPlan) : DomainEvent;
