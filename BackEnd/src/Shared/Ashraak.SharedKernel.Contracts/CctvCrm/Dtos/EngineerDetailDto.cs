namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Engineer detail (GET /cctv/engineers/{id}).</summary>
public sealed record EngineerDetailDto(
    Guid Id,
    string EngineerNumber,
    string Name,
    string Phone,
    string Status,
    Guid? PlatformUserId,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion);
