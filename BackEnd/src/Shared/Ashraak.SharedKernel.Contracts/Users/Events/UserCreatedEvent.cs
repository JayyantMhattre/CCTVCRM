using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.SharedKernel.Contracts.Users.Events;

/// <summary>
/// Raised by the Users module when a new user profile has been created.
/// Handled by the Audit module to record the registration, and potentially
/// by a Notification module to send a welcome or verification email.
/// </summary>
/// <param name="UserId">The newly created user's identifier.</param>
/// <param name="TenantId">The tenant the user belongs to.</param>
/// <param name="Email">The user's registered email address.</param>
/// <param name="DisplayName">The user's display name at the time of creation.</param>
public sealed record UserCreatedEvent(
    Guid UserId,
    Guid TenantId,
    string Email,
    string DisplayName) : DomainEvent;
