using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetEngineerDashboard;

internal sealed class GetEngineerDashboardQueryHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IEngineerLookupService engineerLookup,
    ITicketLookupService ticketLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetEngineerDashboardQuery, Result<EngineerDashboardDto>>
{
    public async Task<Result<EngineerDashboardDto>> Handle(
        GetEngineerDashboardQuery request,
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

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var schedules = await scheduleRepository.GetForEngineerAsync(engineer.Id, today, today, cancellationToken);

        var scheduleSummaries = new List<ScheduleSummaryDto>();
        foreach (var schedule in schedules)
        {
            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            scheduleSummaries.Add(ServiceMapper.ToScheduleSummary(schedule, visit));
        }

        var openTickets = await ticketLookup.GetOpenTicketsForEngineerAsync(engineer.Id, cancellationToken);

        return new EngineerDashboardDto(
            engineer.Id,
            scheduleSummaries.Count,
            openTickets.Count,
            scheduleSummaries,
            openTickets);
    }
}
