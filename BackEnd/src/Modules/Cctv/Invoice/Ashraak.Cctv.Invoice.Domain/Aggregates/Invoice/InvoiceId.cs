namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Strongly typed invoice identifier.</summary>
public readonly record struct InvoiceId(Guid Value)
{
    public static InvoiceId New() => new(Guid.NewGuid());

    public static InvoiceId From(Guid value) => new(value);
}
