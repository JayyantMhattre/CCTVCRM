namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Request body for PATCH /cctv/sites/{siteId}/status.</summary>
public sealed record ChangeSiteStatusRequest(
    string Status,
    uint RowVersion);
