using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Repositories;

internal sealed class AmcContractRepository(AmcDbContext db) : IAmcContractRepository
{
    private IQueryable<ContractAggregate> QueryWithChildren() =>
        db.AmcContracts
            .Include(c => c.Terms)
            .Include(c => c.Documents);

    public Task<ContractAggregate?> GetByIdAsync(AmcContractId id, CancellationToken cancellationToken) =>
        QueryWithChildren().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<ContractAggregate?> GetActiveBySiteIdAsync(Guid siteId, CancellationToken cancellationToken) =>
        QueryWithChildren()
            .FirstOrDefaultAsync(c => c.SiteId == siteId && c.Status == ContractStatus.Active, cancellationToken);

    public Task<ContractAggregate?> GetByCustomerIdWithActiveTermAsync(
        Guid customerId,
        CancellationToken cancellationToken) =>
        QueryWithChildren()
            .Where(c => c.CustomerId == customerId && c.Status == ContractStatus.Active)
            .OrderByDescending(c => c.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<AmcContractListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ContractStatus? status,
        Guid? siteId,
        Guid? customerId,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = QueryWithChildren();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);
        if (siteId.HasValue)
            query = query.Where(c => c.SiteId == siteId.Value);
        if (customerId.HasValue)
            query = query.Where(c => c.CustomerId == customerId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.ContractNumber.Contains(term));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new AmcContractListResult(items, totalCount);
    }

    public async Task<AmcRenewalRequestListResult> GetRenewalRequestsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = QueryWithChildren()
            .SelectMany(
                c => c.Terms.Where(t => t.RenewalRequestedByCustomer && t.RenewalRequestedAtUtc != null),
                (contract, term) => new { contract, term });

        var totalCount = await query.LongCountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.term.RenewalRequestedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new AmcRenewalRequestListResult(
            rows.Select(x => (x.contract, x.term)).ToList(),
            totalCount);
    }

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"AMC-{year}-";
        return await db.AmcContracts
            .Where(c => c.ContractNumber.StartsWith(prefix))
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<(ContractAggregate Contract, AmcContractTerm Term)>> GetActiveTermsExpiringOnAsync(
        DateOnly expiryDate,
        CancellationToken cancellationToken)
    {
        var rows = await QueryWithChildren()
            .Where(c => c.Status == ContractStatus.Active)
            .SelectMany(c => c.Terms, (contract, term) => new { contract, term })
            .Where(x => x.term.Status == TermStatus.Active && x.term.EndDate == expiryDate)
            .ToListAsync(cancellationToken);

        return rows.Select(x => (x.contract, x.term)).ToList();
    }

    public void Add(ContractAggregate contract) => db.AmcContracts.Add(contract);
}
