namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Engineer master list row (GET /cctv/engineers).</summary>
public sealed record EngineerSummaryDto(
    Guid Id,
    string EngineerNumber,
    string Name,
    string Phone,
    string Status,
    Guid? PlatformUserId,
    DateTime CreatedAtUtc);
