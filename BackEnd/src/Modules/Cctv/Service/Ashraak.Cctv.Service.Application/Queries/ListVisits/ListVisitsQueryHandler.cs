using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Enums;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.ListVisits;

internal sealed class ListVisitsQueryHandler(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListVisitsQuery, Result<PagedResult<VisitSummaryDto>>>
{
    public async Task<Result<PagedResult<VisitSummaryDto>>> Handle(
        ListVisitsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await VisitAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        VisitReportStatus? reportStatus = null;
        if (!string.IsNullOrWhiteSpace(request.ReportStatus))
        {
            if (!ServiceMapper.TryParseVisitReportStatus(request.ReportStatus, out var parsedStatus))
                return Error.Validation("Visits.InvalidReportStatus", "Invalid visit report status filter.");

            reportStatus = parsedStatus;
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var result = await visitRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            reportStatus,
            request.EngineerId,
            request.SiteId,
            cancellationToken);

        var items = new List<VisitSummaryDto>();
        foreach (var visit in result.Items)
        {
            var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
            if (schedule is not null)
                items.Add(ServiceMapper.ToVisitSummary(visit, schedule));
        }

        return new PagedResult<VisitSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
