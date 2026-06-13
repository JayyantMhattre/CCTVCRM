namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>PUT /cctv/invoices/{id} request body.</summary>
public sealed record UpdateInvoiceDraftRequest(
    Guid? SiteId,
    string InvoiceType,
    Guid? AmcContractTermId,
    Guid? TicketId,
    Guid? ServiceVisitId,
    DateOnly InvoiceDate,
    DateOnly? DueDate,
    IReadOnlyList<InvoiceLineRequest> Lines,
    decimal? TaxAmount,
    uint RowVersion);
