using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Users.Domain.Aggregates.UserProfile.Events;

/// <summary>
/// Raised by <see cref="UserProfile.Deactivate"/> when a user profile is deactivated.
/// Handled by the Auth module to revoke active tokens for the deactivated user.
/// </summary>
/// <param name="UserId">The identifier of the deactivated user.</param>
/// <param name="TenantId">The tenant context in which the deactivation occurred.</param>
public sealed record UserDeactivatedDomainEvent(Guid UserId, Guid TenantId) : DomainEvent;
