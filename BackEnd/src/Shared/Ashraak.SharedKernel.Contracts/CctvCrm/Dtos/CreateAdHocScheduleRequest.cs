namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/schedules — ad-hoc schedule within active term.</summary>
public sealed record CreateAdHocScheduleRequest(
    Guid ContractTermId,
    Guid SiteId,
    DateOnly ScheduledDate,
    string? Notes);
