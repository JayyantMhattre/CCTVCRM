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

namespace Ashraak.Cctv.Invoice.Application.Commands.UpdateInvoiceDraft;

internal sealed class UpdateInvoiceDraftCommandHandler(
    IInvoiceRepository invoiceRepository,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateInvoiceDraftCommand, Result<InvoiceDetailDto>>
{
    public async Task<Result<InvoiceDetailDto>> Handle(
        UpdateInvoiceDraftCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

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

        if (!InvoiceMapper.TryParseType(request.InvoiceType, out var invoiceType))
            return Error.Validation("Invoices.InvalidType", "Invalid invoice type.");

        if (request.SiteId.HasValue)
        {
            var site = await siteLookup.GetSiteAsync(request.SiteId.Value, cancellationToken);
            if (site is null)
                return Error.NotFound("Sites.NotFound", "Site not found.");

            if (site.CustomerId != invoice.CustomerId)
                return Error.Validation("Invoices.SiteCustomerMismatch", "Site does not belong to the invoice customer.");
        }

        if (InvoiceMapper.RequiresAmcTerm(invoiceType) && !request.AmcContractTermId.HasValue)
            return Error.Validation("Invoices.AmcTermRequired", "AMC contract term is required for this invoice type.");

        try
        {
            var taxAmount = InvoiceMapper.ResolveTaxAmount(request.Lines, request.TaxAmount);
            var lineDrafts = InvoiceMapper.ToLineDrafts(request.Lines);

            invoice.UpdateDraft(
                request.SiteId,
                invoiceType,
                request.AmcContractTermId,
                request.TicketId,
                request.ServiceVisitId,
                request.InvoiceDate,
                request.DueDate,
                lineDrafts,
                taxAmount,
                request.UserId);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return InvoiceMapper.ToDetail(invoice);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Invoices.UpdateFailed", ex.Message);
        }
    }
}
