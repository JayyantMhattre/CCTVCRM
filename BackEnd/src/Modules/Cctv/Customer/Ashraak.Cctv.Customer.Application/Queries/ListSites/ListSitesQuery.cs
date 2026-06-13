using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.ListSites;

public sealed record ListSitesQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    Guid? CustomerId,
    string? Status,
    string? Search) : IRequest<Result<PagedResult<SiteSummaryDto>>>;
