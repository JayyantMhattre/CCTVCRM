using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Customer.Infrastructure.Services;

internal sealed class SiteLookupService(
    ISiteRepository siteRepository,
    ICustomerRepository customerRepository) : ISiteLookupService
{
    public async Task<SiteSummaryDto?> GetSiteAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        var site = await siteRepository.GetByIdAsync(SiteId.From(siteId), cancellationToken);
        return site is null ? null : SiteMapper.ToSummary(site);
    }

    public async Task<IReadOnlyList<SiteSummaryDto>> GetSitesForCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var sites = await siteRepository.GetByCustomerIdAsync(CustomerId.From(customerId), cancellationToken);
        return sites.Select(SiteMapper.ToSummary).ToList();
    }

    public async Task<bool> ValidateSiteOwnershipAsync(
        Guid siteId,
        Guid portalUserId,
        CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByPortalUserIdAsync(portalUserId, cancellationToken);
        if (customer is null)
            return false;

        var site = await siteRepository.GetByIdAsync(SiteId.From(siteId), cancellationToken);
        return site is not null && site.CustomerId == customer.Id;
    }
}
