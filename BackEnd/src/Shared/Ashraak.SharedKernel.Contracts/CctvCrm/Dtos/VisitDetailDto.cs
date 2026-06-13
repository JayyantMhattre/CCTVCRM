namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Visit detail with full evidence checklist (GET /cctv/visits/{id}).</summary>
public sealed record VisitDetailDto(
    Guid Id,
    Guid ServiceScheduleId,
    string ScheduleNumber,
    Guid SiteId,
    Guid EngineerId,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    string? VisitRemarks,
    string ReportStatus,
    bool IsCustomerVisible,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion,
    bool HasSelfie,
    bool HasGps,
    bool HasBeforeDuringAfterPhoto,
    bool HasSignature,
    bool HasMinimumRemarks,
    IReadOnlyList<VisitPhotoDto> Photos,
    VisitLocationDto? Location,
    VisitSignatureDto? Signature,
    IReadOnlyList<VisitApprovalDto> Approvals,
    IReadOnlyList<VisitAttachmentDto> Attachments);
