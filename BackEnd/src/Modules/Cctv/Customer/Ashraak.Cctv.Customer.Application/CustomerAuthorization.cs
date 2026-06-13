using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Customer.Application;

internal static class CustomerAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, CustomerPermissions.Read, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, CustomerPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Customers.ReadForbidden", "You do not have permission to read customers.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, CustomerPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Customers.ManageForbidden", "You do not have permission to manage customers.");
    }

    public static async Task<Error?> EnsureCanAccessPortalProfileAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken))
            return null;

        return Error.Forbidden(
            "Customers.PortalForbidden",
            "Only customer portal users can access their profile.");
    }
}
