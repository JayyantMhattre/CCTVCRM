using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant.Events;

/// <summary>
/// Raised by <see cref="Tenant.Suspend"/> when a tenant account is suspended.
/// Handled by the Audit module to record the action and, optionally, by a Notification
/// module to alert the tenant owner.
/// </summary>
/// <param name="TenantId">The identifier of the suspended tenant.</param>
/// <param name="Reason">The administrative reason for the suspension.</param>
public sealed record TenantSuspendedDomainEvent(Guid TenantId, string Reason) : DomainEvent;
