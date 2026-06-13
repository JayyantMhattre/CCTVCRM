using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Commands.CreateTicket;

public sealed record CreateTicketCommand(
    Guid TenantId,
    Guid UserId,
    Guid SiteId,
    string Subject,
    string Description,
    string Priority,
    Guid? ServiceVisitId,
    IReadOnlyList<Guid>? AttachmentFileIds) : IRequest<Result<TicketDetailDto>>;
