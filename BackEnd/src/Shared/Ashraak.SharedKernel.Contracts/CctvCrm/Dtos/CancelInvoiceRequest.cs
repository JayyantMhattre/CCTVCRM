namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/invoices/{id}/cancel request body.</summary>
public sealed record CancelInvoiceRequest(
    string Reason,
    uint RowVersion);
