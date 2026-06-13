using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Queries.GetInvoice;

public sealed record GetInvoiceQuery(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId) : IRequest<Result<InvoiceDetailDto>>;
