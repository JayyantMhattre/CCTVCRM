using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetEngineerSchedules;

internal sealed class GetEngineerSchedulesQueryHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetEngineerSchedulesQuery, Result<IReadOnlyList<ScheduleSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<ScheduleSummaryDto>>> Handle(
        GetEngineerSchedulesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var engineer = await engineerLookup.GetForPlatformUserAsync(request.UserId, cancellationToken);
        if (engineer is null)
            return Error.Forbidden("Schedules.EngineerNotFound", "No engineer profile linked to this user.");

        var schedules = await scheduleRepository.GetForEngineerAsync(
            engineer.Id,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var items = new List<ScheduleSummaryDto>();
        foreach (var schedule in schedules)
        {
            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            items.Add(ServiceMapper.ToScheduleSummary(schedule, visit));
        }

        return items;
    }
}
