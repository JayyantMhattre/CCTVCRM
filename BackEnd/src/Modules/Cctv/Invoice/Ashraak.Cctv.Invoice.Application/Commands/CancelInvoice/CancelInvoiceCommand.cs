using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.CancelInvoice;

public sealed record CancelInvoiceCommand(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId,
    string Reason,
    uint RowVersion) : IRequest<Result<InvoiceDetailDto>>;
