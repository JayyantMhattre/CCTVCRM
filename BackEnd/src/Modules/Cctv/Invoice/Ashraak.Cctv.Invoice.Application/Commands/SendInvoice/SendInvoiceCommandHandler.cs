using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.SendInvoice;

internal sealed class SendInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<SendInvoiceCommand, Result<InvoiceDetailDto>>
{
    public async Task<Result<InvoiceDetailDto>> Handle(
        SendInvoiceCommand request,
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

        try
        {
            invoice.Send(request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return InvoiceMapper.ToDetail(invoice);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Invoices.SendFailed", ex.Message);
        }
    }
}
