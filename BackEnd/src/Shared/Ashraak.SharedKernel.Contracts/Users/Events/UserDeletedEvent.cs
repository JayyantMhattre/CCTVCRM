using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Users.Events;

/// <summary>
/// Raised when a user profile is permanently deleted.
/// Handled by the Auth module to remove the corresponding <c>AuthUser</c> record,
/// and by Audit to log the deletion event.
/// </summary>
/// <param name="UserId">The identifier of the deleted user.</param>
/// <param name="TenantId">The tenant from which the user was deleted.</param>
public sealed record UserDeletedEvent(Guid UserId, Guid TenantId) : DomainEvent;
