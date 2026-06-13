using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Append-only status transition audit row.</summary>
public sealed class InvoiceStatusHistory : Entity<InvoiceStatusHistoryId>
{
    private InvoiceStatusHistory(InvoiceStatusHistoryId id) : base(id) { }

    public InvoiceId InvoiceId { get; private set; }
    public InvoiceStatus? FromStatus { get; private set; }
    public InvoiceStatus ToStatus { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }
    public Guid ChangedBy { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static InvoiceStatusHistory Create(
        InvoiceStatusHistoryId id,
        InvoiceId invoiceId,
        InvoiceStatus? fromStatus,
        InvoiceStatus toStatus,
        Guid changedBy)
    {
        return new InvoiceStatusHistory(id)
        {
            InvoiceId = invoiceId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = changedBy,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = changedBy
        };
    }
}
