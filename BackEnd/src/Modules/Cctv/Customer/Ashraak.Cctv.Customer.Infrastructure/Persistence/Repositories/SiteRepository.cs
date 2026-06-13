using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Repositories;

internal sealed class SiteRepository(CustomerDbContext db) : ISiteRepository
{
    private IQueryable<SiteAggregate> SitesWithIncludes =>
        db.Sites
            .Include(s => s.Contacts)
            .Include(s => s.Documents)
            .Include(s => s.AssetSummary);

    public Task<SiteAggregate?> GetByIdAsync(SiteId id, CancellationToken cancellationToken) =>
        SitesWithIncludes.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<SiteAggregate?> GetByNumberAsync(string siteNumber, CancellationToken cancellationToken) =>
        SitesWithIncludes.FirstOrDefaultAsync(s => s.SiteNumber == siteNumber, cancellationToken);

    public async Task<IReadOnlyList<SiteAggregate>> GetByCustomerIdAsync(
        CustomerId customerId,
        CancellationToken cancellationToken) =>
        await SitesWithIncludes
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<SiteListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CustomerId? customerId,
        SiteStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = db.Sites.AsQueryable();

        if (customerId.HasValue)
            query = query.Where(s => s.CustomerId == customerId.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(s =>
                s.SiteNumber.Contains(term)
                || s.Name.Contains(term)
                || s.Address.Contains(term)
                || s.City.Contains(term));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new SiteListResult(items, totalCount);
    }

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"ST-{year}-";
        return await db.Sites
            .IgnoreQueryFilters()
            .Where(s => s.SiteNumber.StartsWith(prefix))
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SiteAggregate>> GetByPortalCustomerAsync(
        Guid portalUserId,
        CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.PortalUserId == portalUserId, cancellationToken);

        if (customer is null)
            return [];

        return await GetByCustomerIdAsync(customer.Id, cancellationToken);
    }

    public void Add(SiteAggregate site) => db.Sites.Add(site);
}
