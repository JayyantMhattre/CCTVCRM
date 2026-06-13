namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Site list item (GET /cctv/sites, GET /cctv/customers/{id}/sites).</summary>
public sealed record SiteSummaryDto(
    Guid Id,
    string SiteNumber,
    Guid CustomerId,
    string Name,
    string Address,
    string City,
    string Status,
    DateTime CreatedAtUtc);
