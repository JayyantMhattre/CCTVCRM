namespace Ashraak.Cctv.Invoice.Domain.Enums;

/// <summary>Invoice lifecycle status (BR-INV-01).</summary>
public enum InvoiceStatus
{
    Draft = 0,
    Generated = 1,
    Sent = 2,
    Paid = 3,
    Cancelled = 4
}
