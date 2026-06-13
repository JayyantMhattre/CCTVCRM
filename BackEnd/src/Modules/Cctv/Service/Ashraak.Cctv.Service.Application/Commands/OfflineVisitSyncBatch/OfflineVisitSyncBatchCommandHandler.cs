using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.OfflineVisitSyncBatch;

internal sealed class OfflineVisitSyncBatchCommandHandler(
    IEngineerLookupService engineerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<OfflineVisitSyncBatchCommand, Result<OfflineSyncResultDto>>
{
    public async Task<Result<OfflineSyncResultDto>> Handle(
        OfflineVisitSyncBatchCommand request,
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

        var accepted = request.Items.Select(i => i.VisitId).Distinct().ToList();
        return new OfflineSyncResultDto(accepted, []);
    }
}
