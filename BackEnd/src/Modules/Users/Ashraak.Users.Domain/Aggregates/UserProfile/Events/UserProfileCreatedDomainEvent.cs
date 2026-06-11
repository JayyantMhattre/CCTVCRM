using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Users.Domain.Aggregates.UserProfile.Events;

/// <summary>
/// Raised by <see cref="UserProfile.Create"/> when a new user profile is created.
/// Handled by the Audit module to record the registration event.
/// Promoted to <c>UserCreatedEvent</c> in <c>SharedKernel.Contracts</c> for cross-module distribution.
/// </summary>
/// <param name="UserId">The newly created user's identifier (shared with Auth module).</param>
/// <param name="TenantId">The tenant the user was added to.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="DisplayName">The user's display name at the time of creation.</param>
public sealed record UserProfileCreatedDomainEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string DisplayName) : DomainEvent;
