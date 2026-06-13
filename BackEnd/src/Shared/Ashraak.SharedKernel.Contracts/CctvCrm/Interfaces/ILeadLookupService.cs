using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Read-only lead lookup for cross-module integration (Customer/Site back-references).</summary>
public interface ILeadLookupService
{
    Task<LeadSummaryDto?> GetByIdAsync(Guid leadId, CancellationToken cancellationToken = default);

    Task<LeadSummaryDto?> GetByNumberAsync(string leadNumber, CancellationToken cancellationToken = default);
}
