namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Strongly typed invoice line identifier.</summary>
public readonly record struct InvoiceLineId(Guid Value)
{
    public static InvoiceLineId New() => new(Guid.NewGuid());

    public static InvoiceLineId From(Guid value) => new(value);
}
