namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Visit GPS capture record.</summary>
public sealed record VisitLocationDto(
    Guid Id,
    decimal Latitude,
    decimal Longitude,
    DateTime CapturedAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
