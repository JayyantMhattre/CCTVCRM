namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/invoices request body.</summary>
public sealed record CreateInvoiceRequest(
    Guid CustomerId,
    Guid? SiteId,
    string InvoiceType,
    Guid? AmcContractTermId,
    Guid? TicketId,
    Guid? ServiceVisitId,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    IReadOnlyList<InvoiceLineRequest> Lines,
    decimal? TaxAmount = null);
