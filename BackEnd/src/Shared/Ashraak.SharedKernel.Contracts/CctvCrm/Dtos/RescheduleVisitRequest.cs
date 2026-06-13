namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/schedules/{scheduleId}/reschedule.</summary>
public sealed record RescheduleVisitRequest(
    DateOnly NewScheduledDate,
    string Reason,
    uint RowVersion);
