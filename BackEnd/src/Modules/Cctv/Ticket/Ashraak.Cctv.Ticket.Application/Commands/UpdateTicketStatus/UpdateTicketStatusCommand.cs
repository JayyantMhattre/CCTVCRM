using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.UpdateTicketStatus;

public sealed record UpdateTicketStatusCommand(
    Guid TenantId,
    Guid UserId,
    Guid TicketId,
    string ToStatus,
    string? Comment,
    uint RowVersion) : IRequest<Result<TicketDetailDto>>;
