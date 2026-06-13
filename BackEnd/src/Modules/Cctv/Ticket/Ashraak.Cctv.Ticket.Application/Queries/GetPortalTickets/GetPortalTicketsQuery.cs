using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Queries.GetPortalTickets;

public sealed record GetPortalTicketsQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<TicketSummaryDto>>>;
