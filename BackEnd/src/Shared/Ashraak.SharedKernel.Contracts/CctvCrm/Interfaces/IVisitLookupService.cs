using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to service visits.</summary>
public interface IVisitLookupService
{
    Task<VisitDetailDto?> GetVisitAsync(
        Guid visitId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VisitSummaryDto>> GetApprovedVisitsForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);
}
