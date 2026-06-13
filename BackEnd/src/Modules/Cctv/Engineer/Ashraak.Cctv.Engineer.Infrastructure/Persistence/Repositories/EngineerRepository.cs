using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Cctv.Engineer.Infrastructure.Persistence.Repositories;

internal sealed class EngineerRepository(EngineerDbContext db) : IEngineerRepository
{
    public Task<EngineerAggregate?> GetByIdAsync(EngineerId id, CancellationToken cancellationToken) =>
        db.Engineers.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<EngineerAggregate?> GetByPlatformUserIdAsync(Guid platformUserId, CancellationToken cancellationToken) =>
        db.Engineers.FirstOrDefaultAsync(e => e.PlatformUserId == platformUserId, cancellationToken);

    public Task<IReadOnlyList<EngineerAggregate>> GetAllActiveAsync(CancellationToken cancellationToken) =>
        db.Engineers
            .Where(e => e.Status == EngineerStatus.Active)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<EngineerAggregate>)t.Result, cancellationToken);

    public async Task<EngineerListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        EngineerStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = db.Engineers.AsQueryable();

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e =>
                e.EngineerNumber.Contains(term)
                || e.Name.Contains(term)
                || e.Phone.Contains(term));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new EngineerListResult(items, totalCount);
    }

    public Task<bool> ExistsByPlatformUserIdAsync(
        Guid platformUserId,
        EngineerId? excludeId,
        CancellationToken cancellationToken)
    {
        var query = db.Engineers.Where(e => e.PlatformUserId == platformUserId);
        if (excludeId is not null)
            query = query.Where(e => e.Id != excludeId);

        return query.AnyAsync(cancellationToken);
    }

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"EN-{year}-";
        return await db.Engineers
            .Where(e => e.EngineerNumber.StartsWith(prefix))
            .CountAsync(cancellationToken);
    }

    public void Add(EngineerAggregate engineer) => db.Engineers.Add(engineer);
}
