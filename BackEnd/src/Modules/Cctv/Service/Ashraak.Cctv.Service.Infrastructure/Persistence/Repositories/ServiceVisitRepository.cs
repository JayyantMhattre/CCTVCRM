using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Enums;
using Ashraak.Cctv.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using VisitAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Visit.ServiceVisit;

namespace Ashraak.Cctv.Service.Infrastructure.Persistence.Repositories;

internal sealed class ServiceVisitRepository(ServiceDbContext db) : IServiceVisitRepository
{
    public Task<VisitAggregate?> GetByIdAsync(ServiceVisitId id, CancellationToken cancellationToken) =>
        db.ServiceVisits
            .Include(v => v.Photos)
            .Include(v => v.Approvals)
            .Include(v => v.Attachments)
            .Include(v => v.Location)
            .Include(v => v.Signature)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public Task<VisitAggregate?> GetByScheduleIdAsync(ServiceScheduleId scheduleId, CancellationToken cancellationToken) =>
        db.ServiceVisits
            .Include(v => v.Photos)
            .Include(v => v.Approvals)
            .Include(v => v.Attachments)
            .Include(v => v.Location)
            .Include(v => v.Signature)
            .FirstOrDefaultAsync(v => v.ServiceScheduleId == scheduleId, cancellationToken);

    public async Task<VisitListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        VisitReportStatus? reportStatus,
        Guid? engineerId,
        Guid? siteId,
        CancellationToken cancellationToken)
    {
        var query = db.ServiceVisits
            .Include(v => v.Photos)
            .AsQueryable();

        if (reportStatus.HasValue)
            query = query.Where(v => v.ReportStatus == reportStatus.Value);
        if (engineerId.HasValue)
            query = query.Where(v => v.EngineerId == engineerId.Value);
        if (siteId.HasValue)
        {
            var scheduleIds = db.ServiceSchedules
                .Where(s => s.SiteId == siteId.Value)
                .Select(s => s.Id);
            query = query.Where(v => scheduleIds.Contains(v.ServiceScheduleId));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(v => v.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new VisitListResult(items, totalCount);
    }

    public Task<IReadOnlyList<VisitAggregate>> GetPendingApprovalsAsync(CancellationToken cancellationToken) =>
        db.ServiceVisits
            .Where(v => v.ReportStatus == VisitReportStatus.Submitted)
            .OrderBy(v => v.CompletedAtUtc)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<VisitAggregate>)t.Result, cancellationToken);

    public async Task<IReadOnlyList<VisitAggregate>> GetApprovedForSitesAsync(
        IReadOnlyList<Guid> siteIds,
        CancellationToken cancellationToken)
    {
        var scheduleIds = await db.ServiceSchedules
            .Where(s => siteIds.Contains(s.SiteId))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        return await db.ServiceVisits
            .Where(v => scheduleIds.Contains(v.ServiceScheduleId))
            .Where(v => v.ReportStatus == VisitReportStatus.Approved && v.IsCustomerVisible)
            .OrderByDescending(v => v.CompletedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void Add(VisitAggregate visit) => db.ServiceVisits.Add(visit);
}
