using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.ListPendingApprovals;

internal sealed class ListPendingApprovalsQueryHandler(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListPendingApprovalsQuery, Result<IReadOnlyList<VisitSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<VisitSummaryDto>>> Handle(
        ListPendingApprovalsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await VisitAuthorization.EnsureCanApproveAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var visits = await visitRepository.GetPendingApprovalsAsync(cancellationToken);
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
