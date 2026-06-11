using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Auth.Application.Commands.RegisterUser;

/// <summary>
/// Command to register a new user identity in the Auth module.
/// On success, creates an <c>AuthUser</c> aggregate with a hashed password and
/// returns the new user's <see cref="Guid"/> identifier.
/// </summary>
/// <param name="TenantId">
/// The tenant the user will be registered under. The tenant must be active;
/// the handler returns <c>403 Forbidden</c> if the tenant is suspended or not found.
/// </param>
/// <param name="Email">
/// The user's email address. Must be unique within the tenant.
/// Returns <c>409 Conflict</c> if already taken.
/// </param>
/// <param name="Password">
/// The plain-text password. Hashed by <c>IPasswordHasher</c> before storage.
/// Minimum 8 characters, must include uppercase, lowercase, and a digit.
/// </param>
/// <param name="DisplayName">
/// The user's display name stored in the Users module's <c>UserProfile</c>.
/// Maximum 100 characters.
/// </param>
public sealed record RegisterUserCommand(
    Guid TenantId,
    string Email,
    string Password,
    string DisplayName) : IRequest<Result<Guid>>;
