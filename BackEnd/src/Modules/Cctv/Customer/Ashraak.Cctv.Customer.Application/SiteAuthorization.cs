using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Customer.Application;

internal static class SiteAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, SitePermissions.Read, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, SitePermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Sites.ReadForbidden", "You do not have permission to read sites.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, SitePermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Sites.ManageForbidden", "You do not have permission to manage sites.");
    }

    public static async Task<Error?> EnsureCanAccessPortalSitesAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken))
            return null;

        return Error.Forbidden(
            "Sites.PortalForbidden",
            "Only customer portal users can access their sites.");
    }
}
