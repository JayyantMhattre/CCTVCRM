namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Strongly typed invoice attachment identifier.</summary>
public readonly record struct InvoiceAttachmentId(Guid Value)
{
    public static InvoiceAttachmentId New() => new(Guid.NewGuid());

    public static InvoiceAttachmentId From(Guid value) => new(value);
}
