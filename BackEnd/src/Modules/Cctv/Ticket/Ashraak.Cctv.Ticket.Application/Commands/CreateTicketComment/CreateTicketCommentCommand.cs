using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.CreateTicketComment;

public sealed record CreateTicketCommentCommand(
    Guid TenantId,
    Guid UserId,
    Guid TicketId,
    string Text) : IRequest<Result<TicketCommentDto>>;
