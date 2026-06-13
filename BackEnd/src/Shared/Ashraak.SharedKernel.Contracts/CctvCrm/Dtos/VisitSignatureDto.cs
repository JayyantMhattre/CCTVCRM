namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Customer signature metadata.</summary>
public sealed record VisitSignatureDto(
    Guid Id,
    Guid FileId,
    string SignedByName,
    DateTime CapturedAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
