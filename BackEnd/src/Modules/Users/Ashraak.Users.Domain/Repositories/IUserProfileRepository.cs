using Ashraak.Users.Domain.Aggregates.UserProfile;

namespace Ashraak.Users.Domain.Repositories;

/// <summary>
/// Repository port for the <see cref="UserProfile"/> aggregate root.
/// Implemented by <c>UserProfileRepository</c> in the Users Infrastructure layer.
/// </summary>
public interface IUserProfileRepository
{
    /// <summary>Returns the <see cref="UserProfile"/> with the given <paramref name="id"/>, or <see langword="null"/>.</summary>
    /// <param name="id">The strongly-typed user identifier.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<UserProfile?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the <see cref="UserProfile"/> matching <paramref name="email"/> within <paramref name="tenantId"/>,
    /// or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="email">Email address to match (case-insensitive).</param>
    /// <param name="tenantId">The tenant scope.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<UserProfile?> GetByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all <see cref="UserProfile"/> records belonging to <paramref name="tenantId"/>.
    /// Used when deactivating all users after a tenant is deleted.
    /// </summary>
    /// <param name="tenantId">The tenant whose users to retrieve.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<IReadOnlyList<UserProfile>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when a <see cref="UserProfile"/> exists for the given
    /// <paramref name="userId"/> within <paramref name="tenantId"/>.
    /// </summary>
    /// <param name="userId">The user identifier to check.</param>
    /// <param name="tenantId">The tenant scope.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> ExistsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Tracks a new <see cref="UserProfile"/> for insertion.</summary>
    void Add(UserProfile profile);

    /// <summary>Marks an existing <see cref="UserProfile"/> as modified for update.</summary>
    void Update(UserProfile profile);
}
