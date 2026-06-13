namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Visit attachment metadata (video, report PDF).</summary>
public sealed record VisitAttachmentDto(
    Guid Id,
    Guid FileId,
    string AttachmentType,
    string? Title,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
