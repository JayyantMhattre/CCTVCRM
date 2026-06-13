namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Site detail (GET /cctv/sites/{id}).</summary>
public sealed record SiteDetailDto(
    Guid Id,
    string SiteNumber,
    Guid CustomerId,
    string Name,
    string Address,
    string City,
    string Status,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion);
