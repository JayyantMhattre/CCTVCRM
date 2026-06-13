namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract;

public readonly record struct AmcContractTermId(Guid Value)
{
    public static AmcContractTermId New() => new(Guid.NewGuid());
    public static AmcContractTermId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
