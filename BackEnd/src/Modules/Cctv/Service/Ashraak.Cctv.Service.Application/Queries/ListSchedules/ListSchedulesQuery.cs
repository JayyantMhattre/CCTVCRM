using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.ListSchedules;

public sealed record ListSchedulesQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? Status,
    Guid? EngineerId,
    Guid? SiteId) : IRequest<Result<PagedResult<ScheduleSummaryDto>>>;
