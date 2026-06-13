using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to engineer master (D1-5 stub for assignment validation).</summary>
public interface IEngineerLookupService
{
    Task<bool> ExistsAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default);

    Task<EngineerSummaryDto?> GetAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default);

    Task<EngineerSummaryDto?> GetForPlatformUserAsync(
        Guid platformUserId,
        CancellationToken cancellationToken = default);
}
