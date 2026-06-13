namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/selfie.</summary>
public sealed record LinkVisitSelfieRequest(
    Guid FileId,
    DateTime? CapturedAt);
