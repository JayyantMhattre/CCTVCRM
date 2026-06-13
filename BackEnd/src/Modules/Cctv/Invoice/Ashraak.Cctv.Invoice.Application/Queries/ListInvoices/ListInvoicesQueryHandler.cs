using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.ListInvoices;

internal sealed class ListInvoicesQueryHandler(
    IInvoiceRepository invoiceRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListInvoicesQuery, Result<PagedResult<InvoiceSummaryDto>>>
{
    public async Task<Result<PagedResult<InvoiceSummaryDto>>> Handle(
        ListInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

        var authError = await InvoiceAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (!await InvoiceAuthorization.IsAdminAsync(permissionChecker, request.UserId, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.ListForbidden", "Admin access required for invoice list.");

        InvoiceStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!InvoiceMapper.TryParseStatus(request.Status, out var parsedStatus))
                return Error.Validation("Invoices.InvalidStatus", "Invalid invoice status filter.");

            status = parsedStatus;
        }

        InvoiceType? invoiceType = null;
        if (!string.IsNullOrWhiteSpace(request.InvoiceType))
        {
            if (!InvoiceMapper.TryParseType(request.InvoiceType, out var parsedType))
                return Error.Validation("Invoices.InvalidType", "Invalid invoice type filter.");

            invoiceType = parsedType;
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var result = await invoiceRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            status,
            invoiceType,
            request.CustomerId,
            request.Search,
            cancellationToken);

        var items = result.Items.Select(InvoiceMapper.ToSummary).ToList();
        return new PagedResult<InvoiceSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
