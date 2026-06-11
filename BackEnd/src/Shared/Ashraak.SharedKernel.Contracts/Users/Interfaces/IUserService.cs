using Ashraak.SharedKernel.Contracts.Users.Dtos;

namespace Ashraak.SharedKernel.Contracts.Users.Interfaces;

/// <summary>
/// Cross-module facade for reading user data from the Users module.
/// Implemented by <c>UserService</c> in <c>Ashraak.Users.Infrastructure</c>.
/// </summary>
/// <remarks>
/// Used by Auth and Audit modules to resolve user information without taking a
/// direct dependency on the Users module's database. Implementations should
/// cache responses in Redis to avoid repeated database round trips.
/// </remarks>
public interface IUserService
{
    /// <summary>
    /// Returns a <see cref="UserDto"/> for the given <paramref name="userId"/>,
    /// or <see langword="null"/> if no matching user profile exists.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all user profiles belonging to the specified <paramref name="tenantId"/>.
    /// Used by admin screens to list tenant members.
    /// </summary>
    /// <param name="tenantId">The tenant whose members should be listed.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<IReadOnlyList<UserDto>> GetUsersForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when a user with <paramref name="userId"/> exists
    /// and belongs to <paramref name="tenantId"/>.
    /// Used as a precondition check before creating resources that reference a user.
    /// </summary>
    /// <param name="userId">The user identifier to check.</param>
    /// <param name="tenantId">The expected tenant of the user.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> ExistsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}
