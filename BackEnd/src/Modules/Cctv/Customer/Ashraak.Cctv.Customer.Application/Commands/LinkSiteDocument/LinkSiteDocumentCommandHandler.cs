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

namespace Ashraak.Cctv.Customer.Application.Commands.LinkSiteDocument;

internal sealed class LinkSiteDocumentCommandHandler(
    ISiteRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<LinkSiteDocumentCommand, Result<SiteDocumentDto>>
{
    public async Task<Result<SiteDocumentDto>> Handle(
        LinkSiteDocumentCommand request,
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
            var documentType = SiteMapper.ParseDocumentType(request.DocumentType);
            var document = site.LinkDocument(request.FileId, documentType, request.Title, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return SiteMapper.ToDocument(document);
        }
        catch (ArgumentException ex)
        {
            return Error.Validation("Sites.InvalidDocumentType", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Sites.InvalidState", ex.Message);
        }
    }
}
