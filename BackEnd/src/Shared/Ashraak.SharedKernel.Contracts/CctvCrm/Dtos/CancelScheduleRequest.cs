namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/schedules/{scheduleId}/cancel.</summary>
public sealed record CancelScheduleRequest(
    string Reason,
    uint RowVersion);
