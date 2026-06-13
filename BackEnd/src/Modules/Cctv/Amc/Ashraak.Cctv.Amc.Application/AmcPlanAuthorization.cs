using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Amc.Application;

internal static class AmcPlanAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, AmcPlanPermissions.Read, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, AmcPlanPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("AmcPlans.ReadForbidden", "You do not have permission to read AMC plans.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, AmcPlanPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("AmcPlans.ManageForbidden", "You do not have permission to manage AMC plans.");
    }
}
