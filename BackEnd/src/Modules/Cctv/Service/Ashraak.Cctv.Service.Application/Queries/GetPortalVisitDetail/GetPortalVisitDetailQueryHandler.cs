using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetPortalVisitDetail;

internal sealed class GetPortalVisitDetailQueryHandler(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalVisitDetailQuery, Result<VisitDetailDto>>
{
    public async Task<Result<VisitDetailDto>> Handle(
        GetPortalVisitDetailQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await VisitAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var visit = await visitRepository.GetByIdAsync(ServiceVisitId.From(request.VisitId), cancellationToken);
        if (visit is null)
            return Error.NotFound("Visits.NotFound", "Visit not found.");

        if (!visit.IsCustomerVisible)
            return Error.NotFound("Visits.NotVisible", "Visit report is not available.");

        var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
        if (schedule is null)
            return Error.NotFound("Schedules.NotFound", "Schedule not found.");

        if (!await siteLookup.ValidateSiteOwnershipAsync(schedule.SiteId, request.UserId, cancellationToken))
            return Error.Forbidden("Visits.PortalForbidden", "You do not have access to this visit.");

        return ServiceMapper.ToVisitDetail(visit, schedule);
    }
}
