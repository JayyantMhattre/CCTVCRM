namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

public readonly record struct SiteAssetSummaryId(Guid Value)
{
    public static SiteAssetSummaryId New() => new(Guid.NewGuid());

    public static SiteAssetSummaryId From(Guid value) => new(value);
}
