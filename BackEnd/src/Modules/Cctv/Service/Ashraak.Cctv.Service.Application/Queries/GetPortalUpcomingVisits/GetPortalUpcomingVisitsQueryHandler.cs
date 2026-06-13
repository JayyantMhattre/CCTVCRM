using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetPortalUpcomingVisits;

internal sealed class GetPortalUpcomingVisitsQueryHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    ICustomerLookupService customerLookup,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalUpcomingVisitsQuery, Result<IReadOnlyList<ScheduleSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<ScheduleSummaryDto>>> Handle(
        GetPortalUpcomingVisitsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
        if (customer is null)
            return Error.NotFound("Schedules.PortalCustomerNotFound", "No customer profile linked to this user.");

        var sites = await siteLookup.GetSitesForCustomerAsync(customer.Id, cancellationToken);
        if (sites.Count == 0)
            return Array.Empty<ScheduleSummaryDto>();

        var siteIds = sites.Select(s => s.Id).ToList();
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var schedules = await scheduleRepository.GetUpcomingForSitesAsync(siteIds, fromDate, cancellationToken);

        var items = new List<ScheduleSummaryDto>();
        foreach (var schedule in schedules)
        {
            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            items.Add(ServiceMapper.ToScheduleSummary(schedule, visit));
        }

        return items;
    }
}
