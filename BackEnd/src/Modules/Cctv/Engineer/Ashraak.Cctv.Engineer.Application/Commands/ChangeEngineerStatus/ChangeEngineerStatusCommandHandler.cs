using Ashraak.Cctv.Engineer.Application.Mapping;
using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Commands.ChangeEngineerStatus;

internal sealed class ChangeEngineerStatusCommandHandler(
    IEngineerRepository repository,
    IScheduleLookupService scheduleLookup,
    ITicketLookupService ticketLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeEngineerStatusCommand, Result<EngineerDetailDto>>
{
    private static readonly HashSet<string> ActiveScheduleStatuses =
    [
        "Planned",
        "Assigned",
        "InProgress"
    ];

    public async Task<Result<EngineerDetailDto>> Handle(
        ChangeEngineerStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.EngineersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Engineers.Disabled", "Engineer management is not enabled for this tenant.");

        var authError = await EngineerAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var engineer = await repository.GetByIdAsync(EngineerId.From(request.EngineerId), cancellationToken);
        if (engineer is null)
            return Error.NotFound("Engineers.NotFound", "Engineer not found.");

        var concurrencyError = EngineerConcurrencyHelper.EnsureRowVersion(engineer, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        EngineerStatus toStatus;
        try
        {
            toStatus = EngineerMapper.ParseStatus(request.Status);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Engineers.InvalidStatus", "Invalid engineer status.");
        }

        if (toStatus == EngineerStatus.Inactive && engineer.IsActive)
        {
            var assignmentError = await EnsureNoActiveAssignmentsAsync(engineer.Id.Value, cancellationToken);
            if (assignmentError is not null)
                return assignmentError;
        }

        engineer.ChangeStatus(toStatus, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return EngineerMapper.ToDetail(engineer);
    }

    private async Task<Error?> EnsureNoActiveAssignmentsAsync(
        Guid engineerId,
        CancellationToken cancellationToken)
    {
        var schedules = await scheduleLookup.GetSchedulesForEngineerAsync(
            engineerId,
            null,
            null,
            cancellationToken);
        if (schedules.Any(s => ActiveScheduleStatuses.Contains(s.Status)))
        {
            return Error.Conflict(
                "Engineers.ActiveAssignments",
                "Cannot deactivate an engineer with active schedule assignments.");
        }

        var tickets = await ticketLookup.GetOpenTicketsForEngineerAsync(engineerId, cancellationToken);
        if (tickets.Count > 0)
        {
            return Error.Conflict(
                "Engineers.ActiveAssignments",
                "Cannot deactivate an engineer with open ticket assignments.");
        }

        return null;
    }
}
