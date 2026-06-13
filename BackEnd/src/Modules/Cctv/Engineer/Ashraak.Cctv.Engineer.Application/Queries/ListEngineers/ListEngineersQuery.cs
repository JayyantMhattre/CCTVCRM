using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Queries.ListEngineers;

public sealed record ListEngineersQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? Status,
    string? Search) : IRequest<Result<PagedResult<EngineerSummaryDto>>>;
