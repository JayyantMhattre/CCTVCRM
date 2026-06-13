namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Visit list row.</summary>
public sealed record VisitSummaryDto(
    Guid Id,
    Guid ServiceScheduleId,
    string ScheduleNumber,
    Guid SiteId,
    Guid EngineerId,
    string ReportStatus,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    bool IsCustomerVisible,
    DateTime CreatedAtUtc);
