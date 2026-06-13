namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Strongly typed invoice status history identifier.</summary>
public readonly record struct InvoiceStatusHistoryId(Guid Value)
{
    public static InvoiceStatusHistoryId New() => new(Guid.NewGuid());

    public static InvoiceStatusHistoryId From(Guid value) => new(value);
}
