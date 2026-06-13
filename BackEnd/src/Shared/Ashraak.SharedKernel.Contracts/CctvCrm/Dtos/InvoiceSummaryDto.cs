namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Invoice list row.</summary>
public sealed record InvoiceSummaryDto(
    Guid Id,
    string InvoiceNumber,
    Guid CustomerId,
    Guid? SiteId,
    string InvoiceType,
    string Status,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    decimal TotalAmount,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);
