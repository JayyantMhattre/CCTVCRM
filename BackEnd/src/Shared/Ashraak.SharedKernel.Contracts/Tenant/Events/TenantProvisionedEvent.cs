using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Tenant.Events;

/// <summary>
/// Raised by the Tenant module when a new tenant has been fully provisioned.
/// Handled by the Users module to create the owner's <c>UserProfile</c>, by the
/// Audit module to record the provisioning action, and potentially by the
/// Notification module (future) to send a welcome email.
/// </summary>
/// <param name="TenantId">The unique identifier of the newly created tenant.</param>
/// <param name="Name">The display name of the tenant.</param>
/// <param name="Slug">The URL-safe slug assigned to the tenant.</param>
/// <param name="Plan">The initial subscription plan.</param>
/// <param name="OwnerUserId">The <c>AuthUser</c> identifier of the account owner.</param>
public sealed record TenantProvisionedEvent(
    Guid TenantId,
    string Name,
    string Slug,
    TenantPlan Plan,
    Guid OwnerUserId) : DomainEvent;
