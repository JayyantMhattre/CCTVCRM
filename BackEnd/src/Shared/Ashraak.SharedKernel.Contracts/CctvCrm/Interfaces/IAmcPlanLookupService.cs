using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to AMC plans and versions.</summary>
public interface IAmcPlanLookupService
{
    Task<AmcPlanVersionDetailDto?> GetPublishedVersionAsync(
        Guid planVersionId,
        CancellationToken cancellationToken = default);

    Task<bool> IsPlanVersionPublishedAsync(
        Guid planVersionId,
        CancellationToken cancellationToken = default);
}
