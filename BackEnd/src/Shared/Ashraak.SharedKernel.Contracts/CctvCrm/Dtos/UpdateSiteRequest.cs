namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Request body for PUT /cctv/sites/{siteId}.</summary>
public sealed record UpdateSiteRequest(
    string Name,
    string Address,
    string City,
    uint RowVersion);
