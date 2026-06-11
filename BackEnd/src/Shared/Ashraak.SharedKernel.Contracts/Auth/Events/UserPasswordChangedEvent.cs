using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Auth.Events;

/// <summary>
/// Raised after a user's password has been successfully changed or reset.
/// Handled by the Auth module's token revocation handler to invalidate all existing
/// sessions, and by the Audit module to record the security event.
/// </summary>
/// <param name="UserId">The user whose password was changed.</param>
/// <param name="TenantId">The tenant context of the change.</param>
public sealed record UserPasswordChangedEvent(Guid UserId, Guid TenantId) : DomainEvent;
