using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Ticket.Application;

internal static class TicketAuthorization
{
    public static async Task<Error?> EnsureCanReadAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, TicketPermissions.Read, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Engineer", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Tickets.ReadForbidden", "You do not have permission to read tickets.");
    }

    public static async Task<Error?> EnsureCanCreateAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, TicketPermissions.Create, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Engineer", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Tickets.CreateForbidden", "You do not have permission to create tickets.");
    }

    public static async Task<Error?> EnsureCanAssignAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, TicketPermissions.Assign, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Tickets.AssignForbidden", "You do not have permission to assign tickets.");
    }

    public static async Task<Error?> EnsureCanUpdateAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, TicketPermissions.Update, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Engineer", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Tickets.UpdateForbidden", "You do not have permission to update tickets.");
    }

    public static async Task<Error?> EnsureCanCloseAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, TicketPermissions.Close, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Admin", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Tickets.CloseForbidden", "You do not have permission to close tickets.");
    }

    public static async Task<Error?> EnsureCanReopenAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        if (await permissionChecker.HasPermissionAsync(userId, tenantId, TicketPermissions.Reopen, cancellationToken)
            || await permissionChecker.IsInRoleAsync(userId, tenantId, "Customer", cancellationToken))
        {
            return null;
        }

        return Error.Forbidden("Tickets.ReopenForbidden", "You do not have permission to reopen tickets.");
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

    public static async Task<bool> IsEngineerAsync(
        IAuthPermissionChecker permissionChecker,
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken) =>
        await permissionChecker.IsInRoleAsync(userId, tenantId, "Engineer", cancellationToken);
}
