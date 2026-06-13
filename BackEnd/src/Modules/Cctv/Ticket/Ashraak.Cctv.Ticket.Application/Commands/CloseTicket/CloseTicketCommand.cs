using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.CloseTicket;

public sealed record CloseTicketCommand(
    Guid TenantId,
    Guid UserId,
    Guid TicketId,
    uint RowVersion) : IRequest<Result<TicketDetailDto>>;
