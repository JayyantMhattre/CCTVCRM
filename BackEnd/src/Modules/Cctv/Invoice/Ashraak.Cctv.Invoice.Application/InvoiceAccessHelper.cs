using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Results;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Cctv.Invoice.Application;

internal static class InvoiceAccessHelper
{
    public static async Task<Error?> EnsureInvoiceAccessAsync(
        InvoiceAggregate invoice,
        Guid userId,
        Guid tenantId,
        IAuthPermissionChecker permissionChecker,
        ICustomerLookupService customerLookup,
        CancellationToken cancellationToken)
    {
        if (await InvoiceAuthorization.IsAdminAsync(permissionChecker, userId, tenantId, cancellationToken))
            return null;

        if (!await InvoiceAuthorization.IsCustomerAsync(permissionChecker, userId, tenantId, cancellationToken))
            return Error.Forbidden("Invoices.AccessDenied", "You do not have access to this invoice.");

        var customer = await customerLookup.GetCustomerForUserAsync(userId, cancellationToken);
        if (customer is null || customer.Id != invoice.CustomerId)
            return Error.Forbidden("Invoices.AccessDenied", "You can only access your own invoices.");

        return null;
    }
}
