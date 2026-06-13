using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.GetInvoicePdf;

public sealed record GetInvoicePdfQuery(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId) : IRequest<Result<InvoicePdfDto>>;
