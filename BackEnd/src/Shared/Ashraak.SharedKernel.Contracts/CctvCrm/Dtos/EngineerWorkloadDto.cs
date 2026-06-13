namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Active assignments for an engineer (GET /cctv/engineers/{id}/workload).</summary>
public sealed record EngineerWorkloadDto(
    Guid EngineerId,
    int ActiveScheduleCount,
    int OpenTicketCount,
    IReadOnlyList<ScheduleSummaryDto> ActiveSchedules,
    IReadOnlyList<TicketSummaryDto> OpenTickets);
