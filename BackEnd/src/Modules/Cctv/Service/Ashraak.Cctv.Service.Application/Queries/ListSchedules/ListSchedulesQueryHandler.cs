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

namespace Ashraak.Cctv.Service.Application.Queries.ListSchedules;

internal sealed class ListSchedulesQueryHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListSchedulesQuery, Result<PagedResult<ScheduleSummaryDto>>>
{
    public async Task<Result<PagedResult<ScheduleSummaryDto>>> Handle(
        ListSchedulesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        ScheduleStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!ServiceMapper.TryParseScheduleStatus(request.Status, out var parsedStatus))
                return Error.Validation("Schedules.InvalidStatus", "Invalid schedule status filter.");

            status = parsedStatus;
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var result = await scheduleRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            request.FromDate,
            request.ToDate,
            status,
            request.EngineerId,
            request.SiteId,
            cancellationToken);

        var items = new List<ScheduleSummaryDto>();
        foreach (var schedule in result.Items)
        {
            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            items.Add(ServiceMapper.ToScheduleSummary(schedule, visit));
        }

        return new PagedResult<ScheduleSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
