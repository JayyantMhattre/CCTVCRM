namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract;

public readonly record struct AmcContractDocumentId(Guid Value)
{
    public static AmcContractDocumentId New() => new(Guid.NewGuid());
    public static AmcContractDocumentId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
