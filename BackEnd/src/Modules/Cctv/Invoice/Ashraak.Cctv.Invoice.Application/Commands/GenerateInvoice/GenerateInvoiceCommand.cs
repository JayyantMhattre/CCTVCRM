using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Invoice.Application.Commands.GenerateInvoice;

public sealed record GenerateInvoiceCommand(
    Guid TenantId,
    Guid UserId,
    Guid InvoiceId,
    uint RowVersion) : IRequest<Result<GenerateInvoiceResultDto>>;
