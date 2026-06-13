using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Enums;

namespace Ashraak.Cctv.Service.Domain.Repositories;

public sealed record ScheduleListResult(
    IReadOnlyList<ServiceSchedule> Items,
    long TotalCount);

public interface IServiceScheduleRepository
{
    Task<ServiceSchedule?> GetByIdAsync(ServiceScheduleId id, CancellationToken cancellationToken);

    Task<ServiceSchedule?> GetByNumberAsync(string scheduleNumber, CancellationToken cancellationToken);

    Task<int> CountByTermAsync(Guid amcContractTermId, CancellationToken cancellationToken);

    Task<ScheduleListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        ScheduleStatus? status,
        Guid? engineerId,
        Guid? siteId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceSchedule>> GetForEngineerAsync(
        Guid engineerId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceSchedule>> GetUpcomingForSitesAsync(
        IReadOnlyList<Guid> siteIds,
        DateOnly fromDate,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    void Add(ServiceSchedule schedule);

    void AddRange(IEnumerable<ServiceSchedule> schedules);
}
