using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateSiteAssetSummary;

internal sealed class UpdateSiteAssetSummaryCommandHandler(
    ISiteRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateSiteAssetSummaryCommand, Result<SiteAssetSummaryDto>>
{
    public async Task<Result<SiteAssetSummaryDto>> Handle(
        UpdateSiteAssetSummaryCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Sites.Disabled", "Site management is not enabled for this tenant.");

        var authError = await SiteAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var site = await repository.GetByIdAsync(SiteId.From(request.SiteId), cancellationToken);
        if (site is null)
            return Error.NotFound("Sites.NotFound", "Site not found.");

        var concurrencyError = SiteConcurrencyHelper.EnsureRowVersion(site, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            site.UpsertAssetSummary(
                request.CameraCount,
                request.DvrCount,
                request.NvrCount,
                request.HardDiskCount,
                request.SwitchCount,
                request.RouterCount,
                request.MonitorCount,
                request.Brand,
                request.Model,
                request.Remarks,
                request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Sites.AssetSummaryInvalid", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return site.AssetSummary is null
            ? Error.NotFound("Sites.AssetSummaryNotFound", "Asset summary not found.")
            : SiteMapper.ToAssetSummary(site.AssetSummary);
    }
}
