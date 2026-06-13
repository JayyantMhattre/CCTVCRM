using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.GetInvoice;

internal sealed class GetInvoiceQueryHandler(
    IInvoiceRepository invoiceRepository,
    ICustomerLookupService customerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetInvoiceQuery, Result<InvoiceDetailDto>>
{
    public async Task<Result<InvoiceDetailDto>> Handle(
        GetInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

        var authError = await InvoiceAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var invoice = await invoiceRepository.GetByIdAsync(InvoiceId.From(request.InvoiceId), cancellationToken);
        if (invoice is null)
            return Error.NotFound("Invoices.NotFound", "Invoice not found.");

        var accessError = await InvoiceAccessHelper.EnsureInvoiceAccessAsync(
            invoice, request.UserId, request.TenantId, permissionChecker, customerLookup, cancellationToken);
        if (accessError is not null)
            return accessError;

        return InvoiceMapper.ToDetail(invoice);
    }
}
