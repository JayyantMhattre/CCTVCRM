using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.AssignTicket;

public sealed record AssignTicketCommand(
    Guid TenantId,
    Guid UserId,
    Guid TicketId,
    Guid EngineerId,
    uint RowVersion) : IRequest<Result<TicketDetailDto>>;
