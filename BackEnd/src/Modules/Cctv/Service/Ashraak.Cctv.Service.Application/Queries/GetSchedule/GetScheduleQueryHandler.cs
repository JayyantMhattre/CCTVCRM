using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetSchedule;

internal sealed class GetScheduleQueryHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetScheduleQuery, Result<ScheduleDetailDto>>
{
    public async Task<Result<ScheduleDetailDto>> Handle(
        GetScheduleQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var schedule = await scheduleRepository.GetByIdAsync(ServiceScheduleId.From(request.ScheduleId), cancellationToken);
        if (schedule is null)
            return Error.NotFound("Schedules.NotFound", "Schedule not found.");

        var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
        return ServiceMapper.ToScheduleDetail(schedule, visit);
    }
}
