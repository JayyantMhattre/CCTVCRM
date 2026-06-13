using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(
    Guid TenantId,
    Guid UserId,
    Guid CustomerId,
    Guid? SiteId,
    string InvoiceType,
    Guid? AmcContractTermId,
    Guid? TicketId,
    Guid? ServiceVisitId,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    IReadOnlyList<InvoiceLineRequest> Lines,
    decimal? TaxAmount) : IRequest<Result<InvoiceDetailDto>>;
