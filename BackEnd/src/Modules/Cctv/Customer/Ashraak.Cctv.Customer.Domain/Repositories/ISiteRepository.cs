using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Enums;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Cctv.Customer.Domain.Repositories;

public sealed record SiteListResult(IReadOnlyList<SiteAggregate> Items, long TotalCount);

public interface ISiteRepository
{
    Task<SiteAggregate?> GetByIdAsync(SiteId id, CancellationToken cancellationToken);

    Task<SiteAggregate?> GetByNumberAsync(string siteNumber, CancellationToken cancellationToken);

    Task<IReadOnlyList<SiteAggregate>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken);

    Task<SiteListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CustomerId? customerId,
        SiteStatus? status,
        string? search,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    Task<IReadOnlyList<SiteAggregate>> GetByPortalCustomerAsync(
        Guid portalUserId,
        CancellationToken cancellationToken);

    void Add(SiteAggregate site);
}
