using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Read-only site lookup for cross-module integration.</summary>
public interface ISiteLookupService
{
    Task<SiteSummaryDto?> GetSiteAsync(Guid siteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SiteSummaryDto>> GetSitesForCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateSiteOwnershipAsync(
        Guid siteId,
        Guid portalUserId,
        CancellationToken cancellationToken = default);
}
