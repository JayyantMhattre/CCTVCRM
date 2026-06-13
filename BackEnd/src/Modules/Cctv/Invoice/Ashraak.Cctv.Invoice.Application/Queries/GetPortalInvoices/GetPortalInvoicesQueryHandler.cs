using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.GetPortalInvoices;

internal sealed class GetPortalInvoicesQueryHandler(
    IInvoiceRepository invoiceRepository,
    ICustomerLookupService customerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalInvoicesQuery, Result<PagedResult<InvoiceSummaryDto>>>
{
    public async Task<Result<PagedResult<InvoiceSummaryDto>>> Handle(
        GetPortalInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

        if (!await InvoiceAuthorization.IsCustomerAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken)
            && !await InvoiceAuthorization.IsAdminAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken))
        {
            return Error.Forbidden("Invoices.PortalForbidden", "Customer portal access required.");
        }

        var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
        if (customer is null)
            return new PagedResult<InvoiceSummaryDto>([], 1, 20, 0);

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var result = await invoiceRepository.GetForCustomerAsync(
            customer.Id, pageNumber, pageSize, cancellationToken);

        var items = result.Items.Select(InvoiceMapper.ToSummary).ToList();
        return new PagedResult<InvoiceSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
