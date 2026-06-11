namespace Ashraak.SharedKernel.Contracts.Auth.Interfaces;

/// <summary>
/// Result of validating an access or refresh token.
/// </summary>
/// <param name="IsValid">
/// <see langword="true"/> if the token is cryptographically valid, unexpired, and not revoked.
/// </param>
/// <param name="UserId">The subject of the token (empty GUID when <see cref="IsValid"/> is <see langword="false"/>).</param>
/// <param name="TenantId">The tenant claim extracted from the token.</param>
/// <param name="Roles">The roles embedded in the token claims.</param>
public sealed record TokenValidationResult(bool IsValid, Guid UserId, Guid TenantId, IReadOnlyList<string> Roles);

/// <summary>
/// Cross-module facade for token validation and revocation.
/// Implemented by the Auth module's infrastructure and consumed by middleware and
/// other modules that need to check or invalidate authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Validates the given <paramref name="token"/> and returns its claims if valid.
    /// Checks cryptographic signature, expiry, and the Redis token revocation list.
    /// </summary>
    /// <param name="token">The raw JWT or opaque token string from the <c>Authorization</c> header.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    /// <returns>A <see cref="TokenValidationResult"/> indicating validity and claims.</returns>
    Task<TokenValidationResult> ValidateAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all active tokens issued to <paramref name="userId"/> within <paramref name="tenantId"/>.
    /// Adds the user to the Redis token revocation list so that existing tokens are rejected
    /// at the validation step, even before they expire naturally.
    /// </summary>
    /// <param name="userId">The user whose tokens should be revoked.</param>
    /// <param name="tenantId">The tenant scope of the revocation.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task RevokeUserTokensAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}
