using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence.Repositories;

internal sealed class AmcPlanRepository(AmcDbContext db) : IAmcPlanRepository
{
    public Task<PlanAggregate?> GetByIdAsync(AmcPlanId id, CancellationToken cancellationToken) =>
        db.AmcPlans
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<PlanAggregate?> GetByCodeAsync(string planCode, CancellationToken cancellationToken) =>
        db.AmcPlans
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.PlanCode == planCode.Trim().ToUpperInvariant(), cancellationToken);

    public async Task<AmcPlanVersion?> GetVersionByIdAsync(AmcPlanVersionId id, CancellationToken cancellationToken)
    {
        var version = await db.AmcPlanVersions.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
        if (version is null)
            return null;

        await db.AmcPlans
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Id == version.PlanId, cancellationToken);

        return version;
    }

    public Task<bool> IsVersionReferencedAsync(AmcPlanVersionId id, CancellationToken cancellationToken) =>
        db.AmcContractTerms.AnyAsync(t => t.PlanVersionId == id, cancellationToken);

    public async Task<AmcPlanListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        PlanStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = db.AmcPlans.Include(p => p.Versions).AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.PlanCode.Contains(term)
                || p.Name.Contains(term)
                || (p.Description != null && p.Description.Contains(term)));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new AmcPlanListResult(items, totalCount);
    }

    public void Add(PlanAggregate plan) => db.AmcPlans.Add(plan);
}
