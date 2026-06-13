using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.LinkTicketAttachment;

public sealed record LinkTicketAttachmentCommand(
    Guid TenantId,
    Guid UserId,
    Guid TicketId,
    Guid FileId,
    string? Title) : IRequest<Result<TicketAttachmentDto>>;
