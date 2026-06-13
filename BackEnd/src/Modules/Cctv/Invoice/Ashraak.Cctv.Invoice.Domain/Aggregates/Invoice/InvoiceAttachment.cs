using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Invoice PDF or supporting file reference.</summary>
public sealed class InvoiceAttachment : Entity<InvoiceAttachmentId>
{
    private InvoiceAttachment(InvoiceAttachmentId id) : base(id) { }

    public InvoiceId InvoiceId { get; private set; }
    public Guid FileId { get; private set; }
    public InvoiceAttachmentType AttachmentType { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static InvoiceAttachment Create(
        InvoiceAttachmentId id,
        InvoiceId invoiceId,
        Guid fileId,
        InvoiceAttachmentType attachmentType,
        Guid createdBy)
    {
        if (fileId == Guid.Empty)
            throw new InvalidOperationException("File id is required.");

        return new InvoiceAttachment(id)
        {
            InvoiceId = invoiceId,
            FileId = fileId,
            AttachmentType = attachmentType,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
