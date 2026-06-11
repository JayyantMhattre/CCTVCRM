namespace Ashraak.SharedKernel.Contracts.Auth.Interfaces;

/// <summary>
/// Cross-module service for evaluating RBAC (role-based) and ABAC (attribute-based)
/// permissions at runtime.
/// Implemented by the Auth module and consumed by any module that needs to enforce
/// authorisation beyond what the JWT claims carry.
/// </summary>
/// <remarks>
/// Permissions follow the convention <c>"Module.Resource.Action"</c>,
/// e.g. <c>"Tenant.Subscription.Upgrade"</c>, <c>"Users.Profile.Delete"</c>.
/// Results are cached in Redis per user+tenant pair to avoid repeated database lookups.
/// </remarks>
public interface IAuthPermissionChecker
{
    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="userId"/> holds the specified
    /// fine-grained <paramref name="permission"/> within <paramref name="tenantId"/>.
    /// </summary>
    /// <param name="userId">The user to check.</param>
    /// <param name="tenantId">The tenant scope of the check.</param>
    /// <param name="permission">Permission in <c>"Module.Resource.Action"</c> format.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> HasPermissionAsync(Guid userId, Guid tenantId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="userId"/> has been assigned the
    /// given <paramref name="role"/> within <paramref name="tenantId"/>.
    /// </summary>
    /// <param name="userId">The user to check.</param>
    /// <param name="tenantId">The tenant scope of the check.</param>
    /// <param name="role">Role name (e.g. <c>"TenantAdmin"</c>, <c>"Viewer"</c>).</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<bool> IsInRoleAsync(Guid userId, Guid tenantId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all role names currently assigned to <paramref name="userId"/> within
    /// <paramref name="tenantId"/>. Used by token issuance to enrich JWT role claims.
    /// </summary>
    /// <param name="userId">The user whose roles should be resolved.</param>
    /// <param name="tenantId">The tenant scope.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the full list of permissions assigned to <paramref name="userId"/> within
    /// <paramref name="tenantId"/>. Used to hydrate the JWT claims on token issuance.
    /// </summary>
    /// <param name="userId">The user whose permissions should be retrieved.</param>
    /// <param name="tenantId">The tenant scope.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task<IReadOnlyList<string>> GetPermissionsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}
