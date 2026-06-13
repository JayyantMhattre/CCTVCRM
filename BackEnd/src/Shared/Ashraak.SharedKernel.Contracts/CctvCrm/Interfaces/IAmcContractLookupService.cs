using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to AMC contracts.</summary>
public interface IAmcContractLookupService
{
    Task<AmcContractDetailDto?> GetByIdAsync(
        Guid contractId,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveContractForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);

    Task<AmcContractDetailDto?> GetActiveContractForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);

    Task<AmcContractTermDetailDto?> GetTermByIdAsync(
        Guid contractId,
        Guid termId,
        CancellationToken cancellationToken = default);
}
