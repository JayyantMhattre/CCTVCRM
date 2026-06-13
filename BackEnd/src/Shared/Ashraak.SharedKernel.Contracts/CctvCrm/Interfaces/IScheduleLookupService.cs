using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to service schedules.</summary>
public interface IScheduleLookupService
{
    Task<ScheduleDetailDto?> GetScheduleAsync(
        Guid scheduleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ScheduleSummaryDto>> GetSchedulesForEngineerAsync(
        Guid engineerId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default);
}
