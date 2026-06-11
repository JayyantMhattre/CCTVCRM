using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Users.Application.Commands.CreateUserProfile;

/// <summary>
/// Command to create a <c>UserProfile</c> in the Users module.
/// Typically dispatched reactively when the Auth module publishes
/// <c>UserRegisteredEvent</c> after identity creation.
/// </summary>
/// <param name="UserId">
/// The user's shared identifier — must match the corresponding <c>AuthUser.Id</c>.
/// </param>
/// <param name="TenantId">The tenant the user belongs to.</param>
/// <param name="Email">The user's email address (copied from Auth for read convenience).</param>
/// <param name="DisplayName">The user's display name.</param>
public sealed record CreateUserProfileCommand(
    Guid UserId,
    Guid TenantId,
    string Email,
    string DisplayName) : IRequest<Result>;
