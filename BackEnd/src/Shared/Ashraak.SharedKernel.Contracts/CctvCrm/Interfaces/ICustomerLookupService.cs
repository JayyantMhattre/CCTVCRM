using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Read-only customer lookup for cross-module integration.</summary>
public interface ICustomerLookupService
{
    Task<CustomerSummaryDto?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<CustomerSummaryDto?> GetCustomerForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
