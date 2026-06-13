using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Enums;

namespace Ashraak.Cctv.Service.Domain.Repositories;

public sealed record VisitListResult(
    IReadOnlyList<ServiceVisit> Items,
    long TotalCount);

public interface IServiceVisitRepository
{
    Task<ServiceVisit?> GetByIdAsync(ServiceVisitId id, CancellationToken cancellationToken);

    Task<ServiceVisit?> GetByScheduleIdAsync(ServiceScheduleId scheduleId, CancellationToken cancellationToken);

    Task<VisitListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        VisitReportStatus? reportStatus,
        Guid? engineerId,
        Guid? siteId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceVisit>> GetPendingApprovalsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceVisit>> GetApprovedForSitesAsync(
        IReadOnlyList<Guid> siteIds,
        CancellationToken cancellationToken);

    void Add(ServiceVisit visit);
}
