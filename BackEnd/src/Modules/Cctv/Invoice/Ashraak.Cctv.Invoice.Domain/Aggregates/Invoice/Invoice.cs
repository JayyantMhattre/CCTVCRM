using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Events;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;

/// <summary>Invoice aggregate root (schema <c>cctv_invoice.invoices</c>, Option B).</summary>
public sealed class Invoice : AggregateRoot<InvoiceId>
{
    private readonly List<InvoiceLine> _lines = [];
    private readonly List<InvoiceAttachment> _attachments = [];
    private readonly List<InvoiceStatusHistory> _statusHistory = [];

    private Invoice(InvoiceId id) : base(id) { }

    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid? SiteId { get; private set; }
    public InvoiceType InvoiceType { get; private set; }
    public Guid? AmcContractTermId { get; private set; }
    public Guid? TicketId { get; private set; }
    public Guid? ServiceVisitId { get; private set; }
    public DateOnly InvoiceDate { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public decimal SubtotalAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public uint RowVersion { get; private set; }

    public IReadOnlyList<InvoiceLine> Lines => _lines.AsReadOnly();
    public IReadOnlyList<InvoiceAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyList<InvoiceStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public InvoiceAttachment? InvoicePdfAttachment =>
        _attachments.FirstOrDefault(a => a.AttachmentType == InvoiceAttachmentType.InvoicePdf);

    public static Invoice Create(
        InvoiceId id,
        string invoiceNumber,
        Guid customerId,
        Guid? siteId,
        InvoiceType invoiceType,
        Guid? amcContractTermId,
        Guid? ticketId,
        Guid? serviceVisitId,
        DateOnly invoiceDate,
        DateOnly? dueDate,
        IReadOnlyList<LineDraft> lines,
        decimal taxAmount,
        Guid createdBy)
    {
        ValidateAmcTermRequired(invoiceType, amcContractTermId);
        ValidateLines(lines);

        var invoice = new Invoice(id)
        {
            InvoiceNumber = invoiceNumber.Trim(),
            CustomerId = customerId,
            SiteId = siteId,
            InvoiceType = invoiceType,
            AmcContractTermId = amcContractTermId,
            TicketId = ticketId,
            ServiceVisitId = serviceVisitId,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
            Status = InvoiceStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        invoice.ReplaceLines(lines, createdBy);
        invoice.RecordStatusChange(null, InvoiceStatus.Draft, createdBy);

        invoice.RaiseDomainEvent(new InvoiceCreatedDomainEvent(
            id.Value,
            invoice.InvoiceNumber,
            invoiceType.ToString(),
            customerId,
            createdBy));

        return invoice;
    }

    public void UpdateDraft(
        Guid? siteId,
        InvoiceType invoiceType,
        Guid? amcContractTermId,
        Guid? ticketId,
        Guid? serviceVisitId,
        DateOnly invoiceDate,
        DateOnly? dueDate,
        IReadOnlyList<LineDraft> lines,
        decimal taxAmount,
        Guid updatedBy)
    {
        EnsureDraft();

        ValidateAmcTermRequired(invoiceType, amcContractTermId);
        ValidateLines(lines);

        SiteId = siteId;
        InvoiceType = invoiceType;
        AmcContractTermId = amcContractTermId;
        TicketId = ticketId;
        ServiceVisitId = serviceVisitId;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero);

        ReplaceLines(lines, updatedBy);
        Touch(updatedBy);
    }

    public void Generate(Guid pdfFileId, Guid generatedBy)
    {
        EnsureDraft();

        if (_lines.Count == 0)
            throw new InvalidOperationException("At least one line item is required before generating an invoice.");

        if (pdfFileId == Guid.Empty)
            throw new InvalidOperationException("PDF file id is required.");

        if (InvoicePdfAttachment is not null)
            throw new InvalidOperationException("Invoice PDF already exists.");

        _attachments.Add(InvoiceAttachment.Create(
            InvoiceAttachmentId.New(),
            Id,
            pdfFileId,
            InvoiceAttachmentType.InvoicePdf,
            generatedBy));

        ChangeStatus(InvoiceStatus.Generated, generatedBy);

        RaiseDomainEvent(new InvoiceGeneratedDomainEvent(
            Id.Value,
            InvoiceNumber,
            TotalAmount,
            pdfFileId,
            generatedBy));
    }

    public void Send(Guid sentBy)
    {
        if (Status != InvoiceStatus.Generated)
            throw new InvalidOperationException("Only generated invoices can be sent.");

        ChangeStatus(InvoiceStatus.Sent, sentBy);

        RaiseDomainEvent(new InvoiceSentDomainEvent(Id.Value, InvoiceNumber, sentBy));
    }

    public void MarkPaid(DateTime paidAtUtc, Guid markedBy)
    {
        if (Status != InvoiceStatus.Sent)
            throw new InvalidOperationException("Only sent invoices can be marked paid.");

        PaidAtUtc = paidAtUtc;
        ChangeStatus(InvoiceStatus.Paid, markedBy);

        RaiseDomainEvent(new InvoicePaidDomainEvent(Id.Value, InvoiceNumber, paidAtUtc, markedBy));
    }

    public void Cancel(string reason, Guid cancelledBy)
    {
        if (Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled)
            throw new InvalidOperationException($"Invoice in status {Status} cannot be cancelled.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Cancellation reason is required.");

        ChangeStatus(InvoiceStatus.Cancelled, cancelledBy);

        RaiseDomainEvent(new InvoiceCancelledDomainEvent(
            Id.Value,
            InvoiceNumber,
            reason.Trim(),
            cancelledBy));
    }

    private void ReplaceLines(IReadOnlyList<LineDraft> lines, Guid userId)
    {
        _lines.Clear();

        var ordered = lines
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.Description)
            .ToList();

        var lineNo = 1;
        foreach (var draft in ordered)
        {
            _lines.Add(InvoiceLine.Create(
                InvoiceLineId.New(),
                Id,
                lineNo++,
                draft.Description,
                draft.Quantity,
                draft.UnitPrice,
                userId));
        }

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        SubtotalAmount = Math.Round(_lines.Sum(l => l.LineTotal), 2, MidpointRounding.AwayFromZero);
        TotalAmount = Math.Round(SubtotalAmount + TaxAmount, 2, MidpointRounding.AwayFromZero);

        if (SubtotalAmount < 0 || TaxAmount < 0 || TotalAmount < 0)
            throw new InvalidOperationException("Invoice amounts cannot be negative.");

        if (TotalAmount != SubtotalAmount + TaxAmount)
            throw new InvalidOperationException("Total amount must equal subtotal plus tax.");
    }

    private void ChangeStatus(InvoiceStatus toStatus, Guid changedBy)
    {
        var fromStatus = Status;
        Status = toStatus;
        RecordStatusChange(fromStatus, toStatus, changedBy);
        Touch(changedBy);
    }

    private void RecordStatusChange(InvoiceStatus? fromStatus, InvoiceStatus toStatus, Guid changedBy)
    {
        _statusHistory.Add(InvoiceStatusHistory.Create(
            InvoiceStatusHistoryId.New(),
            Id,
            fromStatus,
            toStatus,
            changedBy));
    }

    private void EnsureDraft()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Invoice can only be edited while in Draft status.");
    }

    private static void ValidateAmcTermRequired(InvoiceType invoiceType, Guid? amcContractTermId)
    {
        if (invoiceType is InvoiceType.AmcRenewal or InvoiceType.NewAmc && !amcContractTermId.HasValue)
            throw new InvalidOperationException("AMC contract term is required for AmcRenewal and NewAmc invoice types.");
    }

    private static void ValidateLines(IReadOnlyList<LineDraft> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new InvalidOperationException("At least one line item is required.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }

    /// <summary>Draft line input for create/update.</summary>
    public sealed record LineDraft(
        string Description,
        decimal Quantity,
        decimal UnitPrice,
        int SortOrder);
}
