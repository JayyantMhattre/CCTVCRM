using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetSiteDocuments;

internal sealed class GetSiteDocumentsQueryHandler(
    ISiteRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetSiteDocumentsQuery, Result<IReadOnlyList<SiteDocumentDto>>>
{
    public async Task<Result<IReadOnlyList<SiteDocumentDto>>> Handle(
        GetSiteDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Sites.Disabled", "Site management is not enabled for this tenant.");

        var authError = await SiteAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var site = await repository.GetByIdAsync(SiteId.From(request.SiteId), cancellationToken);
        if (site is null)
            return Error.NotFound("Sites.NotFound", "Site not found.");

        return site.Documents.Where(d => !d.IsDeleted).Select(SiteMapper.ToDocument).ToList();
    }
}
