using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.ApproveVisit;

internal sealed class ApproveVisitCommandHandler(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<ApproveVisitCommand, Result<VisitDetailDto>>
{
    public async Task<Result<VisitDetailDto>> Handle(
        ApproveVisitCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await VisitAuthorization.EnsureCanApproveAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var visit = await visitRepository.GetByIdAsync(ServiceVisitId.From(request.VisitId), cancellationToken);
        if (visit is null)
            return Error.NotFound("Visits.NotFound", "Visit not found.");

        var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
        if (schedule is null)
            return Error.NotFound("Schedules.NotFound", "Schedule not found.");

        try
        {
            visit.Approve(request.UserId, request.ReviewRemarks, schedule.SiteId);
            schedule.MarkCompleted(request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceMapper.ToVisitDetail(visit, schedule);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Visits.ApproveFailed", ex.Message);
        }
    }
}
