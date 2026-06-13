using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Invoice.Application;

internal static class InvoiceAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, InvoicePermissions.Read, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Invoices.ReadForbidden", "You do not have permission to read invoices.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, InvoicePermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Invoices.ManageForbidden", "You do not have permission to manage invoices.");
    }

    public static async Task<Error?> EnsureCanDownloadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, InvoicePermissions.Download, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, InvoicePermissions.Read, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Invoices.DownloadForbidden", "You do not have permission to download invoices.");
    }

    public static async Task<bool> IsAdminAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken) =>
        await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken);

    public static async Task<bool> IsCustomerAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken) =>
        await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken);
}
