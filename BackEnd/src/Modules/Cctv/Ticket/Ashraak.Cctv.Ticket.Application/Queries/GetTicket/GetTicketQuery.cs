using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Queries.GetTicket;

public sealed record GetTicketQuery(
    Guid TenantId,
    Guid UserId,
    Guid TicketId) : IRequest<Result<TicketDetailDto>>;
