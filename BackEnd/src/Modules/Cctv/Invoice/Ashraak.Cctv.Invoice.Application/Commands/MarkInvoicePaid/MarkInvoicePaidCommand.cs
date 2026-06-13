using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.MarkInvoicePaid;

public sealed record MarkInvoicePaidCommand(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId,
    DateTime? PaidAt,
    uint RowVersion) : IRequest<Result<InvoiceDetailDto>>;
