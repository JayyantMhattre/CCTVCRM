using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Webhooks.Application;

internal static class WebhookAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, WebhookPermissions.Read, cancellationToken)
            || await permissionChecker.HasPermissionAsync(userId, tenantId, WebhookPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Webhooks.ReadForbidden", "You do not have permission to read webhooks.");
    }

    public static async Task<Error?> EnsureCanManageAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, WebhookPermissions.Manage, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Webhooks.ManageForbidden", "You do not have permission to manage webhooks.");
    }
}
