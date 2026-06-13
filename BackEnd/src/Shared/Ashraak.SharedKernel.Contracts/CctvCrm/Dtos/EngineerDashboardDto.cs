namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Engineer My Day dashboard (GET /cctv/engineer/dashboard).</summary>
public sealed record EngineerDashboardDto(
    Guid EngineerId,
    int TodayScheduleCount,
    int OpenTicketCount,
    IReadOnlyList<ScheduleSummaryDto> TodaySchedules,
    IReadOnlyList<TicketSummaryDto> OpenTickets);
