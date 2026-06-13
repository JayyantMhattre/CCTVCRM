using Ashraak.Cctv.Invoice.Application.Abstractions;
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
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Cctv.Invoice.Application.Commands.CreateInvoice;

internal sealed class CreateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IInvoiceNumberGenerator numberGenerator,
    ICustomerLookupService customerLookup,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDetailDto>>
{
    public async Task<Result<InvoiceDetailDto>> Handle(
        CreateInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Invoices.Disabled", "Invoice management is not enabled for this tenant.");

        var authError = await InvoiceAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (!InvoiceMapper.TryParseType(request.InvoiceType, out var invoiceType))
            return Error.Validation("Invoices.InvalidType", "Invalid invoice type.");

        var customer = await customerLookup.GetCustomerAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Error.NotFound("Customers.NotFound", "Customer not found.");

        if (request.SiteId.HasValue)
        {
            var site = await siteLookup.GetSiteAsync(request.SiteId.Value, cancellationToken);
            if (site is null)
                return Error.NotFound("Sites.NotFound", "Site not found.");

            if (site.CustomerId != request.CustomerId)
                return Error.Validation("Invoices.SiteCustomerMismatch", "Site does not belong to the specified customer.");
        }

        if (InvoiceMapper.RequiresAmcTerm(invoiceType) && !request.AmcContractTermId.HasValue)
            return Error.Validation("Invoices.AmcTermRequired", "AMC contract term is required for this invoice type.");

        try
        {
            var invoiceNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
            var taxAmount = InvoiceMapper.ResolveTaxAmount(request.Lines, request.TaxAmount);
            var lineDrafts = InvoiceMapper.ToLineDrafts(request.Lines);

            var invoice = InvoiceAggregate.Create(
                InvoiceId.New(),
                invoiceNumber,
                request.CustomerId,
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

            invoiceRepository.Add(invoice);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return InvoiceMapper.ToDetail(invoice);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Invoices.CreateFailed", ex.Message);
        }
    }
}
