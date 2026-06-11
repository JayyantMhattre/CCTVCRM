using Ashraak.SharedKernel.Domain.Events;
using Ashraak.Tenant.Domain.Enums;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant.Events;

/// <summary>
/// Raised by <see cref="Tenant.Create"/> when a new tenant has been provisioned.
/// The outbox processor promotes this to <c>TenantProvisionedEvent</c> (in Contracts)
/// for cross-module distribution to the Users and Audit modules.
/// </summary>
/// <param name="TenantId">The new tenant's unique identifier.</param>
/// <param name="Name">The display name chosen at provisioning.</param>
/// <param name="Slug">The URL slug chosen at provisioning.</param>
/// <param name="Plan">The initial subscription tier.</param>
/// <param name="OwnerUserId">The user identifier of the account owner.</param>
public sealed record TenantCreatedDomainEvent(
    Guid TenantId,
    string Name,
    string Slug,
    TenantPlan Plan,
    Guid OwnerUserId) : DomainEvent;
