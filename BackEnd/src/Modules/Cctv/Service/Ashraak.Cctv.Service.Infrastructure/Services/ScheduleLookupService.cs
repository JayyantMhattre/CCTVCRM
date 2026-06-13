using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Service.Infrastructure.Services;

internal sealed class ScheduleLookupService(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository) : IScheduleLookupService
{
    public async Task<ScheduleDetailDto?> GetScheduleAsync(
        Guid scheduleId,
        CancellationToken cancellationToken = default)
    {
        var schedule = await scheduleRepository.GetByIdAsync(ServiceScheduleId.From(scheduleId), cancellationToken);
        if (schedule is null)
            return null;

        var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
        return ServiceMapper.ToScheduleDetail(schedule, visit);
    }

    public async Task<IReadOnlyList<ScheduleSummaryDto>> GetSchedulesForEngineerAsync(
        Guid engineerId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default)
    {
        var schedules = await scheduleRepository.GetForEngineerAsync(engineerId, fromDate, toDate, cancellationToken);
        var results = new List<ScheduleSummaryDto>();

        foreach (var schedule in schedules)
        {
            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            results.Add(ServiceMapper.ToScheduleSummary(schedule, visit));
        }

        return results;
    }
}
