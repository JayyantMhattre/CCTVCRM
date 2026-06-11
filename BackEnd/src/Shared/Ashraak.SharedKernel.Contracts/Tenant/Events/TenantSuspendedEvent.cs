using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Tenant.Events;

/// <summary>
/// Raised when a tenant account has been suspended (e.g. due to non-payment or a ToS violation).
/// The tenant's users will receive <c>403 Forbidden</c> responses until the account is reinstated.
/// Handled by the Audit module and optionally by a Notification module to alert the tenant owner.
/// </summary>
/// <param name="TenantId">The unique identifier of the suspended tenant.</param>
/// <param name="Reason">A human-readable reason for the suspension (logged and potentially emailed to the owner).</param>
public sealed record TenantSuspendedEvent(
    Guid TenantId,
    string Reason) : DomainEvent;
