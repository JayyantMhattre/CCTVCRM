using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.SendInvoice;

public sealed record SendInvoiceCommand(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId,
    uint RowVersion) : IRequest<Result<InvoiceDetailDto>>;
