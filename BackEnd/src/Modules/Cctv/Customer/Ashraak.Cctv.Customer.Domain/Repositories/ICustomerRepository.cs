using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Enums;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;

namespace Ashraak.Cctv.Customer.Domain.Repositories;

public sealed record CustomerListResult(IReadOnlyList<CustomerAggregate> Items, long TotalCount);

public interface ICustomerRepository
{
    Task<CustomerAggregate?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken);

    Task<CustomerAggregate?> GetByNumberAsync(string customerNumber, CancellationToken cancellationToken);

    Task<CustomerAggregate?> GetByPortalUserIdAsync(Guid portalUserId, CancellationToken cancellationToken);

    Task<CustomerListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CustomerStatus? status,
        string? search,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    void Add(CustomerAggregate customer);
}
