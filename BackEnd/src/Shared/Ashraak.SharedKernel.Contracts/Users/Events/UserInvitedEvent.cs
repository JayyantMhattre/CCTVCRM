using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Users.Events;

/// <summary>
/// Raised when an existing user invites someone new to join their tenant.
/// Handled by a Notification module (future) to dispatch an invitation email
/// containing a link with the <see cref="InvitationToken"/>.
/// The invitation expires at <see cref="ExpiresOnUtc"/>.
/// </summary>
/// <param name="InvitationId">Unique identifier of the invitation record.</param>
/// <param name="InvitationToken">Short-lived, cryptographically random token included in the invite link.</param>
/// <param name="Email">Email address of the invitee.</param>
/// <param name="TenantId">The tenant the invitee is being added to.</param>
/// <param name="InvitedByUserId">The user who initiated the invitation.</param>
/// <param name="ExpiresOnUtc">UTC timestamp at which the invitation token becomes invalid.</param>
public sealed record UserInvitedEvent(
    Guid InvitationId,
    string InvitationToken,
    string Email,
    Guid TenantId,
    Guid InvitedByUserId,
    DateTime ExpiresOnUtc) : DomainEvent;
