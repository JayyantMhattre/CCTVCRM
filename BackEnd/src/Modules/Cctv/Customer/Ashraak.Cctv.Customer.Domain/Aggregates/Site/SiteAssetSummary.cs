using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

/// <summary>Asset count summary for a site (1:1, BR-STRUCT-04/05).</summary>
public sealed class SiteAssetSummary : Entity<SiteAssetSummaryId>
{
    private SiteAssetSummary(SiteAssetSummaryId id) : base(id) { }

    public SiteId SiteId { get; private set; }
    public int CameraCount { get; private set; }
    public int DvrCount { get; private set; }
    public int NvrCount { get; private set; }
    public int HardDiskCount { get; private set; }
    public int SwitchCount { get; private set; }
    public int RouterCount { get; private set; }
    public int MonitorCount { get; private set; }
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public string? Remarks { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public uint RowVersion { get; private set; }

    internal static SiteAssetSummary Create(
        SiteAssetSummaryId id,
        SiteId siteId,
        int cameraCount,
        int dvrCount,
        int nvrCount,
        int hardDiskCount,
        int switchCount,
        int routerCount,
        int monitorCount,
        string? brand,
        string? model,
        string? remarks,
        Guid createdBy)
    {
        EnsureNonNegativeCounts(
            cameraCount, dvrCount, nvrCount, hardDiskCount, switchCount, routerCount, monitorCount);

        return new SiteAssetSummary(id)
        {
            SiteId = siteId,
            CameraCount = cameraCount,
            DvrCount = dvrCount,
            NvrCount = nvrCount,
            HardDiskCount = hardDiskCount,
            SwitchCount = switchCount,
            RouterCount = routerCount,
            MonitorCount = monitorCount,
            Brand = string.IsNullOrWhiteSpace(brand) ? null : brand.Trim(),
            Model = string.IsNullOrWhiteSpace(model) ? null : model.Trim(),
            Remarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };
    }

    internal void Update(
        int cameraCount,
        int dvrCount,
        int nvrCount,
        int hardDiskCount,
        int switchCount,
        int routerCount,
        int monitorCount,
        string? brand,
        string? model,
        string? remarks,
        Guid updatedBy)
    {
        EnsureNonNegativeCounts(
            cameraCount, dvrCount, nvrCount, hardDiskCount, switchCount, routerCount, monitorCount);

        CameraCount = cameraCount;
        DvrCount = dvrCount;
        NvrCount = nvrCount;
        HardDiskCount = hardDiskCount;
        SwitchCount = switchCount;
        RouterCount = routerCount;
        MonitorCount = monitorCount;
        Brand = string.IsNullOrWhiteSpace(brand) ? null : brand.Trim();
        Model = string.IsNullOrWhiteSpace(model) ? null : model.Trim();
        Remarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        RowVersion++;
    }

    private static void EnsureNonNegativeCounts(
        int cameraCount,
        int dvrCount,
        int nvrCount,
        int hardDiskCount,
        int switchCount,
        int routerCount,
        int monitorCount)
    {
        if (cameraCount < 0 || dvrCount < 0 || nvrCount < 0 || hardDiskCount < 0
            || switchCount < 0 || routerCount < 0 || monitorCount < 0)
        {
            throw new InvalidOperationException("Asset counts must be non-negative.");
        }
    }
}
