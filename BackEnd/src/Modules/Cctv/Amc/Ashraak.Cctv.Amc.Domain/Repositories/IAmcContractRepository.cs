using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Enums;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;

namespace Ashraak.Cctv.Amc.Domain.Repositories;

public sealed record AmcContractListResult(IReadOnlyList<ContractAggregate> Items, long TotalCount);

public sealed record AmcRenewalRequestListResult(
    IReadOnlyList<(ContractAggregate Contract, AmcContractTerm Term)> Items,
    long TotalCount);

public interface IAmcContractRepository
{
    Task<ContractAggregate?> GetByIdAsync(AmcContractId id, CancellationToken cancellationToken);

    Task<ContractAggregate?> GetActiveBySiteIdAsync(Guid siteId, CancellationToken cancellationToken);

    Task<ContractAggregate?> GetByCustomerIdWithActiveTermAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<AmcContractListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ContractStatus? status,
        Guid? siteId,
        Guid? customerId,
        string? search,
        CancellationToken cancellationToken);

    Task<AmcRenewalRequestListResult> GetRenewalRequestsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    Task<IReadOnlyList<(ContractAggregate Contract, AmcContractTerm Term)>> GetActiveTermsExpiringOnAsync(
        DateOnly expiryDate,
        CancellationToken cancellationToken);

    void Add(ContractAggregate contract);
}
