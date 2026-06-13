namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract;

public readonly record struct AmcContractId(Guid Value)
{
    public static AmcContractId New() => new(Guid.NewGuid());
    public static AmcContractId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
