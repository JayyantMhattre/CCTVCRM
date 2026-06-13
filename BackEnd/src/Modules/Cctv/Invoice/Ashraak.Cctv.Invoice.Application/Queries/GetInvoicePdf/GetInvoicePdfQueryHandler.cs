using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.GetInvoicePdf;

internal sealed class GetInvoicePdfQueryHandler(
    IInvoiceRepository invoiceRepository,
    ICustomerLookupService customerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetInvoicePdfQuery, Result<InvoicePdfDto>>
{
    public async Task<Result<InvoicePdfDto>> Handle(
        GetInvoicePdfQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

        var authError = await InvoiceAuthorization.EnsureCanDownloadAsync(
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

        var pdfAttachment = invoice.InvoicePdfAttachment;
        if (pdfAttachment is null)
            return Error.NotFound("Invoices.PdfNotFound", "Invoice PDF has not been generated yet.");

        if (pdfAttachment.AttachmentType != InvoiceAttachmentType.InvoicePdf)
            return Error.NotFound("Invoices.PdfNotFound", "Invoice PDF not found.");

        return new InvoicePdfDto(
            invoice.Id.Value,
            invoice.InvoiceNumber,
            pdfAttachment.FileId,
            $"/api/v1/files/{pdfAttachment.FileId}");
    }
}
