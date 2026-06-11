using Ashraak.Auth.Domain.Aggregates.AuthUser;

namespace Ashraak.Auth.Domain.Repositories;

/// <summary>
/// Repository port for the <see cref="AuthUser"/> aggregate root.
/// Implemented by <c>AuthUserRepository</c> in the Auth Infrastructure layer.
/// The interface lives in the Domain layer to enforce dependency inversion.
/// </summary>
public interface IAuthUserRepository
{
    /// <summary>
    /// Returns the <see cref="AuthUser"/> with the given <paramref name="id"/>,
    /// or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="id">The strongly-typed auth user identifier.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<AuthUser?> GetByIdAsync(AuthUserId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the <see cref="AuthUser"/> matching <paramref name="email"/> within <paramref name="tenantId"/>,
    /// or <see langword="null"/> if no match exists.
    /// Used by the login flow to locate the credential record.
    /// </summary>
    /// <param name="email">The email address (compared case-insensitively).</param>
    /// <param name="tenantId">The tenant scope of the lookup.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<AuthUser?> GetByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when an <see cref="AuthUser"/> record already exists
    /// for <paramref name="email"/> within <paramref name="tenantId"/>.
    /// Used during registration to prevent duplicate accounts.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="tenantId">The tenant scope.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> ExistsAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks a new <see cref="AuthUser"/> for insertion on the next <c>SaveChangesAsync</c> call.
    /// </summary>
    /// <param name="user">The new user entity to add.</param>
    void Add(AuthUser user);

    /// <summary>
    /// Marks an existing <see cref="AuthUser"/> as modified for update on the next <c>SaveChangesAsync</c> call.
    /// </summary>
    /// <param name="user">The user entity to update.</param>
    void Update(AuthUser user);
}
