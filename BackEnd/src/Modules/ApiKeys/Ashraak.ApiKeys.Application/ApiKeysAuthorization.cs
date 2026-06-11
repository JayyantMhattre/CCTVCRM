using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.ApiKeys.Application;

internal static class ApiKeysAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, ApiKeysPermissions.Read, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, ApiKeysPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("ApiKeys.ReadForbidden", "You do not have permission to read API keys.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, ApiKeysPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("ApiKeys.ManageForbidden", "You do not have permission to manage API keys.");
    }
}
