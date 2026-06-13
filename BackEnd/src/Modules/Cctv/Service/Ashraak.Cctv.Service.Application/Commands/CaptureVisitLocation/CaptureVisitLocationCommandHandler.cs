using Ashraak.Cctv.Service.Application.Mapping;
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

namespace Ashraak.Cctv.Service.Application.Commands.CaptureVisitLocation;

internal sealed class CaptureVisitLocationCommandHandler(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository,
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CaptureVisitLocationCommand, Result<VisitDetailDto>>
{
    public async Task<Result<VisitDetailDto>> Handle(
        CaptureVisitLocationCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await VisitAuthorization.EnsureCanExecuteAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var engineer = await engineerLookup.GetForPlatformUserAsync(request.UserId, cancellationToken);
        if (engineer is null)
            return Error.Forbidden("Visits.EngineerNotFound", "No engineer profile linked to this user.");

        var visit = await visitRepository.GetByIdAsync(ServiceVisitId.From(request.VisitId), cancellationToken);
        if (visit is null)
            return Error.NotFound("Visits.NotFound", "Visit not found.");

        if (visit.EngineerId != engineer.Id)
            return Error.Forbidden("Visits.NotAssigned", "You are not assigned to this visit.");

        var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
        if (schedule is null)
            return Error.NotFound("Schedules.NotFound", "Schedule not found.");

        try
        {
            visit.CaptureLocation(request.Latitude, request.Longitude, request.CapturedAt, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceMapper.ToVisitDetail(visit, schedule);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Visits.CaptureLocationFailed", ex.Message);
        }
    }
}
