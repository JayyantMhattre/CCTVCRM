namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

public readonly record struct SiteDocumentId(Guid Value)
{
    public static SiteDocumentId New() => new(Guid.NewGuid());

    public static SiteDocumentId From(Guid value) => new(value);
}
