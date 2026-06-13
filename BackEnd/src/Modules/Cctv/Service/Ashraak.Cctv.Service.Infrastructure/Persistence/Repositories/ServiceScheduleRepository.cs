using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Enums;
using Ashraak.Cctv.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ScheduleAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Schedule.ServiceSchedule;

namespace Ashraak.Cctv.Service.Infrastructure.Persistence.Repositories;

internal sealed class ServiceScheduleRepository(ServiceDbContext db) : IServiceScheduleRepository
{
    public Task<ScheduleAggregate?> GetByIdAsync(ServiceScheduleId id, CancellationToken cancellationToken) =>
        db.ServiceSchedules
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<ScheduleAggregate?> GetByNumberAsync(string scheduleNumber, CancellationToken cancellationToken) =>
        db.ServiceSchedules
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.ScheduleNumber == scheduleNumber, cancellationToken);

    public Task<int> CountByTermAsync(Guid amcContractTermId, CancellationToken cancellationToken) =>
        db.ServiceSchedules.CountAsync(s => s.AmcContractTermId == amcContractTermId, cancellationToken);

    public async Task<ScheduleListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        ScheduleStatus? status,
        Guid? engineerId,
        Guid? siteId,
        CancellationToken cancellationToken)
    {
        var query = db.ServiceSchedules.Include(s => s.Assignments).AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(s => s.ScheduledDate >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(s => s.ScheduledDate <= toDate.Value);
        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);
        if (siteId.HasValue)
            query = query.Where(s => s.SiteId == siteId.Value);
        if (engineerId.HasValue)
            query = query.Where(s => s.Assignments.Any(a => a.IsActive && a.EngineerId == engineerId.Value));

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderBy(s => s.ScheduledDate)
            .ThenBy(s => s.ScheduleNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new ScheduleListResult(items, totalCount);
    }

    public Task<IReadOnlyList<ScheduleAggregate>> GetForEngineerAsync(
        Guid engineerId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken) =>
        db.ServiceSchedules
            .Include(s => s.Assignments)
            .Where(s => s.Assignments.Any(a => a.IsActive && a.EngineerId == engineerId))
            .Where(s => !fromDate.HasValue || s.ScheduledDate >= fromDate.Value)
            .Where(s => !toDate.HasValue || s.ScheduledDate <= toDate.Value)
            .OrderBy(s => s.ScheduledDate)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<ScheduleAggregate>)t.Result, cancellationToken);

    public Task<IReadOnlyList<ScheduleAggregate>> GetUpcomingForSitesAsync(
        IReadOnlyList<Guid> siteIds,
        DateOnly fromDate,
        CancellationToken cancellationToken) =>
        db.ServiceSchedules
            .Include(s => s.Assignments)
            .Where(s => siteIds.Contains(s.SiteId))
            .Where(s => s.ScheduledDate >= fromDate)
            .Where(s => s.Status != ScheduleStatus.Cancelled && s.Status != ScheduleStatus.Completed)
            .OrderBy(s => s.ScheduledDate)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<ScheduleAggregate>)t.Result, cancellationToken);

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"VS-{year}-";
        return await db.ServiceSchedules
            .Where(s => s.ScheduleNumber.StartsWith(prefix))
            .CountAsync(cancellationToken);
    }

    public void Add(ScheduleAggregate schedule) => db.ServiceSchedules.Add(schedule);

    public void AddRange(IEnumerable<ScheduleAggregate> schedules) => db.ServiceSchedules.AddRange(schedules);
}
