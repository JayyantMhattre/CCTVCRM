namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Visit photo metadata.</summary>
public sealed record VisitPhotoDto(
    Guid Id,
    Guid FileId,
    string Category,
    string? Caption,
    DateTime CapturedAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
