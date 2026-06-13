using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository(CustomerDbContext db) : ICustomerRepository
{
    public Task<CustomerAggregate?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken) =>
        db.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<CustomerAggregate?> GetByNumberAsync(string customerNumber, CancellationToken cancellationToken) =>
        db.Customers.FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber, cancellationToken);

    public Task<CustomerAggregate?> GetByPortalUserIdAsync(Guid portalUserId, CancellationToken cancellationToken) =>
        db.Customers.FirstOrDefaultAsync(c => c.PortalUserId == portalUserId, cancellationToken);

    public async Task<CustomerListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CustomerStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = db.Customers.AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c =>
                c.CustomerNumber.Contains(term)
                || c.Name.Contains(term)
                || c.Email.Contains(term)
                || c.Phone.Contains(term)
                || c.City.Contains(term));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new CustomerListResult(items, totalCount);
    }

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"CU-{year}-";
        return await db.Customers
            .IgnoreQueryFilters()
            .Where(c => c.CustomerNumber.StartsWith(prefix))
            .CountAsync(cancellationToken);
    }

    public void Add(CustomerAggregate customer) => db.Customers.Add(customer);
}
