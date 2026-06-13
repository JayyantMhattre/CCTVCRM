using Ashraak.Cctv.Lead.Domain.Enums;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence.Repositories;

internal sealed class LeadRepository(LeadDbContext db) : ILeadRepository
{
    public Task<LeadAggregate?> GetByIdAsync(LeadId id, CancellationToken cancellationToken) =>
        db.Leads
            .Include(l => l.Activities)
            .Include(l => l.Remarks)
            .Include(l => l.Attachments)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public Task<LeadAggregate?> GetByNumberAsync(string leadNumber, CancellationToken cancellationToken) =>
        db.Leads.FirstOrDefaultAsync(l => l.LeadNumber == leadNumber, cancellationToken);

    public async Task<LeadListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        LeadStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = db.Leads.AsQueryable();

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(l =>
                l.LeadNumber.Contains(term)
                || l.ContactName.Contains(term)
                || l.Email.Contains(term)
                || l.Phone.Contains(term)
                || (l.OrganizationName != null && l.OrganizationName.Contains(term)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new LeadListResult(items, totalCount);
    }

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"LD-{year}-";
        return await db.Leads
            .IgnoreQueryFilters()
            .Where(l => l.LeadNumber.StartsWith(prefix))
            .CountAsync(cancellationToken);
    }

    public void Add(LeadAggregate lead) => db.Leads.Add(lead);
}
