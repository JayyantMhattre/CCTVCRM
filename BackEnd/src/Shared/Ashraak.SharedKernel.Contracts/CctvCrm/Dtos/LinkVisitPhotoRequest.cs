namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/photos.</summary>
public sealed record LinkVisitPhotoRequest(
    Guid FileId,
    string Category,
    DateTime? CapturedAt);
