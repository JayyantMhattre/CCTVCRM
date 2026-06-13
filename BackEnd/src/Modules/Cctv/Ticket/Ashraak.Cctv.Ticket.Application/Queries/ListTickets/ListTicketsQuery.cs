using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Ticket.Application.Queries.ListTickets;

public sealed record ListTicketsQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? Status,
    string? Priority,
    Guid? CustomerId,
    Guid? SiteId,
    Guid? EngineerId,
    string? Search) : IRequest<Result<PagedResult<TicketSummaryDto>>>;
