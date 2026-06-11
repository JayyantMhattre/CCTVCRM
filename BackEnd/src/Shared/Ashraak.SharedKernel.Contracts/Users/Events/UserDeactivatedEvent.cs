using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Users.Events;

/// <summary>
/// Raised when a user profile is deactivated (user can no longer log in).
/// Handled by the Auth module to revoke active tokens, and by Audit to record the action.
/// </summary>
/// <param name="UserId">The identifier of the deactivated user.</param>
/// <param name="TenantId">The tenant context in which the deactivation occurred.</param>
public sealed record UserDeactivatedEvent(Guid UserId, Guid TenantId) : DomainEvent;
