namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/signature.</summary>
public sealed record LinkVisitSignatureRequest(
    Guid FileId,
    string SignerName,
    DateTime? CapturedAt);
