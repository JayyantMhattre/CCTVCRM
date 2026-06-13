using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Integration.Application.Abstractions;
using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.GenerateInvoice;

internal sealed class GenerateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IPdfGenerationService pdfGenerationService,
    ICctvFileStore fileStore,
    ICustomerLookupService customerLookup,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<GenerateInvoiceCommand, Result<GenerateInvoiceResultDto>>
{
    public async Task<Result<GenerateInvoiceResultDto>> Handle(
        GenerateInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.PdfEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.PdfDisabled", "PDF generation is not enabled for this tenant.");

        var authError = await InvoiceAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var invoice = await invoiceRepository.GetByIdAsync(InvoiceId.From(request.InvoiceId), cancellationToken);
        if (invoice is null)
            return Error.NotFound("Invoices.NotFound", "Invoice not found.");

        var concurrencyError = InvoiceConcurrencyHelper.EnsureRowVersion(request.RowVersion, invoice.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            var customer = await customerLookup.GetCustomerAsync(invoice.CustomerId, cancellationToken);
            SiteSummaryDto? site = invoice.SiteId.HasValue
                ? await siteLookup.GetSiteAsync(invoice.SiteId.Value, cancellationToken)
                : null;

            var model = new
            {
                invoice.InvoiceNumber,
                invoice.CustomerId,
                CustomerName = customer?.Name,
                SiteName = site?.Name,
                invoice.InvoiceType,
                invoice.InvoiceDate,
                invoice.DueDate,
                invoice.SubtotalAmount,
                invoice.TaxAmount,
                invoice.TotalAmount,
                Status = invoice.Status.ToString(),
                Lines = invoice.Lines.Select(l => new
                {
                    l.LineNo,
                    l.Description,
                    l.Quantity,
                    l.UnitPrice,
                    l.LineTotal
                })
            };

            var pdfBytes = await pdfGenerationService.GenerateAsync("invoice", model, cancellationToken);
            var fileName = $"{invoice.InvoiceNumber}.pdf";
            var pdfFileId = await fileStore.StorePdfAsync(
                request.TenantId,
                request.UserId,
                fileName,
                pdfBytes,
                cancellationToken);

            invoice.Generate(pdfFileId, request.UserId);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new GenerateInvoiceResultDto(
                invoice.Id.Value,
                invoice.InvoiceNumber,
                pdfFileId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Invoices.GenerateFailed", ex.Message);
        }
    }
}
