namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Request body for PUT /cctv/sites/{siteId}/asset-summary.</summary>
public sealed record UpdateSiteAssetSummaryRequest(
    int CameraCount,
    int DvrCount,
    int NvrCount,
    int HardDiskCount,
    int SwitchCount,
    int RouterCount,
    int MonitorCount,
    string? Brand,
    string? Model,
    string? Remarks,
    uint RowVersion);
