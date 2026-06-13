using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetPortalSiteDetail;

internal sealed class GetPortalSiteDetailQueryHandler(
    ISiteRepository repository,
    ISiteLookupService siteLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalSiteDetailQuery, Result<SiteDetailDto>>
{
    public async Task<Result<SiteDetailDto>> Handle(
        GetPortalSiteDetailQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Sites.Disabled", "Site management is not enabled for this tenant.");

        var authError = await SiteAuthorization.EnsureCanAccessPortalSitesAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var ownsSite = await siteLookup.ValidateSiteOwnershipAsync(
            request.SiteId, request.UserId, cancellationToken);
        if (!ownsSite)
            return Error.Forbidden("Sites.PortalAccessDenied", "You do not have access to this site.");

        var site = await repository.GetByIdAsync(SiteId.From(request.SiteId), cancellationToken);
        if (site is null)
            return Error.NotFound("Sites.NotFound", "Site not found.");

        return SiteMapper.ToDetail(site);
    }
}
