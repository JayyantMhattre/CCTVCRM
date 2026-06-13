namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

public readonly record struct ServiceVisitId(Guid Value)
{
    public static ServiceVisitId New() => new(Guid.NewGuid());
    public static ServiceVisitId From(Guid value) => new(value);
}
