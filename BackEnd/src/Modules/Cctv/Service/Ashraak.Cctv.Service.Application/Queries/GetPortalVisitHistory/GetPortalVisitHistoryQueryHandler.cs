using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetPortalVisitHistory;

internal sealed class GetPortalVisitHistoryQueryHandler(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository,
    ICustomerLookupService customerLookup,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalVisitHistoryQuery, Result<IReadOnlyList<VisitSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<VisitSummaryDto>>> Handle(
        GetPortalVisitHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await VisitAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
        if (customer is null)
            return Error.NotFound("Visits.PortalCustomerNotFound", "No customer profile linked to this user.");

        var sites = await siteLookup.GetSitesForCustomerAsync(customer.Id, cancellationToken);
        if (sites.Count == 0)
            return Array.Empty<VisitSummaryDto>();

        var siteIds = sites.Select(s => s.Id).ToList();
        var visits = await visitRepository.GetApprovedForSitesAsync(siteIds, cancellationToken);

        var items = new List<VisitSummaryDto>();
        foreach (var visit in visits)
        {
            var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
            if (schedule is not null)
                items.Add(ServiceMapper.ToVisitSummary(visit, schedule));
        }

        return items;
    }
}
