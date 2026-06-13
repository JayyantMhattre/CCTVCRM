using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Queries.GetEngineerTickets;

public sealed record GetEngineerTicketsQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResult<TicketSummaryDto>>>;
