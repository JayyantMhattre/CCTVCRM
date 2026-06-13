using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Billed line item (editable only while parent is Draft).</summary>
public sealed class InvoiceLine : Entity<InvoiceLineId>
{
    private InvoiceLine(InvoiceLineId id) : base(id) { }

    public InvoiceId InvoiceId { get; private set; }
    public int LineNo { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static InvoiceLine Create(
        InvoiceLineId id,
        InvoiceId invoiceId,
        int lineNo,
        string description,
        decimal quantity,
        decimal unitPrice,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidOperationException("Line description is required.");

        if (quantity <= 0)
            throw new InvalidOperationException("Line quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new InvalidOperationException("Line unit price cannot be negative.");

        return new InvoiceLine(id)
        {
            InvoiceId = invoiceId,
            LineNo = lineNo,
            Description = description.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            LineTotal = ComputeLineTotal(quantity, unitPrice),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public static decimal ComputeLineTotal(decimal quantity, decimal unitPrice) =>
        Math.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);
}
