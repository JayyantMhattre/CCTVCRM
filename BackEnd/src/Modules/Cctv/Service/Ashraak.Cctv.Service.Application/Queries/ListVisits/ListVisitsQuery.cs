using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.ListVisits;

public sealed record ListVisitsQuery(
    Guid TenantId,
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? ReportStatus,
    Guid? EngineerId,
    Guid? SiteId) : IRequest<Result<PagedResult<VisitSummaryDto>>>;
