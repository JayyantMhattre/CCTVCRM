namespace Ashraak.Cctv.Customer.Domain.Aggregates.Customer;

/// <summary>Strongly typed identifier for the Customer aggregate.</summary>
public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());

    public static CustomerId From(Guid value) => new(value);
}
