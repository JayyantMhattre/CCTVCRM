using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.CancelSchedule;

internal sealed class CancelScheduleCommandHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelScheduleCommand, Result<ScheduleDetailDto>>
{
    public async Task<Result<ScheduleDetailDto>> Handle(
        CancelScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var schedule = await scheduleRepository.GetByIdAsync(ServiceScheduleId.From(request.ScheduleId), cancellationToken);
        if (schedule is null)
            return Error.NotFound("Schedules.NotFound", "Schedule not found.");

        var concurrencyError = ServiceConcurrencyHelper.EnsureRowVersion(schedule, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            schedule.Cancel(request.Reason, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            return ServiceMapper.ToScheduleDetail(schedule, visit);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Schedules.CancelFailed", ex.Message);
        }
    }
}
