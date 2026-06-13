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

namespace Ashraak.Cctv.Customer.Application.Commands.UpsertSiteContacts;

internal sealed class UpsertSiteContactsCommandHandler(
    ISiteRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpsertSiteContactsCommand, Result<IReadOnlyList<SiteContactDto>>>
{
    public async Task<Result<IReadOnlyList<SiteContactDto>>> Handle(
        UpsertSiteContactsCommand request,
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
            site.ReplaceContacts(SiteMapper.ToContactInputs(request.Contacts), request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Sites.ContactsInvalid", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return site.Contacts.Select(SiteMapper.ToContact).ToList();
    }
}
