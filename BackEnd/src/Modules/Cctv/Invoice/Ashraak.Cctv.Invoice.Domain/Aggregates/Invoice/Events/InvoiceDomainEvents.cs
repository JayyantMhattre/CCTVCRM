using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Events;

public sealed record InvoiceCreatedDomainEvent(
    Guid InvoiceId,
    string InvoiceNumber,
    string InvoiceType,
    Guid CustomerId,
    Guid CreatedBy) : DomainEvent;

public sealed record InvoiceGeneratedDomainEvent(
    Guid InvoiceId,
    string InvoiceNumber,
    decimal TotalAmount,
    Guid PdfFileId,
    Guid GeneratedBy) : DomainEvent;

public sealed record InvoiceSentDomainEvent(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid SentBy) : DomainEvent;

public sealed record InvoicePaidDomainEvent(
    Guid InvoiceId,
    string InvoiceNumber,
    DateTime PaidAtUtc,
    Guid MarkedBy) : DomainEvent;

public sealed record InvoiceCancelledDomainEvent(
    Guid InvoiceId,
    string InvoiceNumber,
    string Reason,
    Guid CancelledBy) : DomainEvent;
