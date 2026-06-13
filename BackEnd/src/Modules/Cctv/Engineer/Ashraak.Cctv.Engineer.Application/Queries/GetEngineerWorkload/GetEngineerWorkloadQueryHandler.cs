using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Queries.GetEngineerWorkload;

internal sealed class GetEngineerWorkloadQueryHandler(
    IEngineerRepository repository,
    IScheduleLookupService scheduleLookup,
    ITicketLookupService ticketLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetEngineerWorkloadQuery, Result<EngineerWorkloadDto>>
{
    private static readonly HashSet<string> ActiveScheduleStatuses =
    [
        "Planned",
        "Assigned",
        "InProgress"
    ];

    public async Task<Result<EngineerWorkloadDto>> Handle(
        GetEngineerWorkloadQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.EngineersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Engineers.Disabled", "Engineer management is not enabled for this tenant.");

        var authError = await EngineerAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var engineer = await repository.GetByIdAsync(EngineerId.From(request.EngineerId), cancellationToken);
        if (engineer is null)
            return Error.NotFound("Engineers.NotFound", "Engineer not found.");

        var schedules = await scheduleLookup.GetSchedulesForEngineerAsync(
            request.EngineerId,
            null,
            null,
            cancellationToken);
        var activeSchedules = schedules
            .Where(s => ActiveScheduleStatuses.Contains(s.Status))
            .ToList();

        var openTickets = await ticketLookup.GetOpenTicketsForEngineerAsync(request.EngineerId, cancellationToken);

        return new EngineerWorkloadDto(
            request.EngineerId,
            activeSchedules.Count,
            openTickets.Count,
            activeSchedules,
            openTickets);
    }
}
