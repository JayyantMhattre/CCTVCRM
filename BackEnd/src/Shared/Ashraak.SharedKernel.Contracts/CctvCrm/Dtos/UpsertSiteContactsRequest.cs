namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Request body for PUT /cctv/sites/{siteId}/contacts.</summary>
public sealed record UpsertSiteContactsRequest(
    IReadOnlyList<SiteContactInputDto> Contacts,
    uint RowVersion);

/// <summary>Contact input within an upsert request.</summary>
public sealed record SiteContactInputDto(
    string Name,
    string? Designation,
    string Phone,
    string? Email,
    bool IsPrimary);
