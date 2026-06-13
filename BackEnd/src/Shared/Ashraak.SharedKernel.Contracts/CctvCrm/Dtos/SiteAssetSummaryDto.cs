namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Asset count summary for a site.</summary>
public sealed record SiteAssetSummaryDto(
    Guid Id,
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
