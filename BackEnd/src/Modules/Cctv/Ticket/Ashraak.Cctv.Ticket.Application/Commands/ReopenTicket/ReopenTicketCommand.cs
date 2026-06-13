using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.ReopenTicket;

public sealed record ReopenTicketCommand(
    Guid TenantId,
    Guid UserId,
    Guid TicketId,
    string Reason,
    uint RowVersion) : IRequest<Result<TicketDetailDto>>;
