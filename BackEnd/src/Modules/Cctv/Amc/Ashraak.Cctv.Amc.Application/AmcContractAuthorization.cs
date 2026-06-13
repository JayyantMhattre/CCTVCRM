using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Amc.Application;

internal static class AmcContractAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, AmcContractPermissions.Read, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, AmcContractPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Amc.ReadForbidden", "You do not have permission to read AMC contracts.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, AmcContractPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Amc.ManageForbidden", "You do not have permission to manage AMC contracts.");
    }

    public static async Task<Error?> EnsureCanRequestRenewalAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, AmcContractPermissions.RequestRenewal, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Amc.RenewalForbidden", "You do not have permission to request AMC renewal.");
    }

    public static async Task<Error?> EnsureCanAccessPortalAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, AmcContractPermissions.Read, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Amc.PortalForbidden", "You do not have permission to access AMC portal data.");
    }
}
