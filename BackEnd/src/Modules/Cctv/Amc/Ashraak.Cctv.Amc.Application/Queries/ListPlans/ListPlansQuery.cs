using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.ListPlans;

public sealed record ListPlansQuery(
    Guid TenantId,
    Guid UserId,
    int Page,
    int PageSize,
    string? Status,
    string? Search) : IRequest<Result<PagedResult<AmcPlanSummaryDto>>>;
