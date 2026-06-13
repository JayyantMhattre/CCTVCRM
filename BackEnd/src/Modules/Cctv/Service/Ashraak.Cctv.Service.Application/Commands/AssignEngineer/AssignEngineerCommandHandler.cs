using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.AssignEngineer;

internal sealed class AssignEngineerCommandHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignEngineerCommand, Result<ScheduleDetailDto>>
{
    public async Task<Result<ScheduleDetailDto>> Handle(
        AssignEngineerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanAssignAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (!await engineerLookup.ExistsAsync(request.EngineerId, cancellationToken))
            return Error.Validation("Schedules.EngineerNotFound", "Engineer not found.");

        var schedule = await scheduleRepository.GetByIdAsync(ServiceScheduleId.From(request.ScheduleId), cancellationToken);
        if (schedule is null)
            return Error.NotFound("Schedules.NotFound", "Schedule not found.");

        var concurrencyError = ServiceConcurrencyHelper.EnsureRowVersion(schedule, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            schedule.AssignEngineer(request.EngineerId, request.UserId);

            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            if (visit is null)
            {
                var assignment = schedule.ActiveAssignment
                    ?? throw new InvalidOperationException("Engineer assignment was not created.");

                visit = ServiceVisit.CreateDraft(
                    ServiceVisitId.New(),
                    schedule.Id,
                    assignment.EngineerId,
                    request.UserId);
                visitRepository.Add(visit);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return ServiceMapper.ToScheduleDetail(schedule, visit);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Schedules.AssignFailed", ex.Message);
        }
    }
}
