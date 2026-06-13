namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Invoice detail with lines, attachments, and status history.</summary>
public sealed record InvoiceDetailDto(
    Guid Id,
    string InvoiceNumber,
    Guid CustomerId,
    Guid? SiteId,
    string InvoiceType,
    Guid? AmcContractTermId,
    Guid? TicketId,
    Guid? ServiceVisitId,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    decimal SubtotalAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    string Status,
    DateTime? PaidAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion,
    IReadOnlyList<InvoiceLineDto> Lines,
    IReadOnlyList<InvoiceAttachmentDto> Attachments,
    IReadOnlyList<InvoiceStatusHistoryDto> StatusHistory);
