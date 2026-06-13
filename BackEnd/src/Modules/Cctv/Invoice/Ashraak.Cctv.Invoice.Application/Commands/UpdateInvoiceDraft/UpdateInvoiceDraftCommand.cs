using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.UpdateInvoiceDraft;

public sealed record UpdateInvoiceDraftCommand(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId,
    Guid? SiteId,
    string InvoiceType,
    Guid? AmcContractTermId,
    Guid? TicketId,
    Guid? ServiceVisitId,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    IReadOnlyList<InvoiceLineRequest> Lines,
    decimal? TaxAmount,
    uint RowVersion) : IRequest<Result<InvoiceDetailDto>>;
