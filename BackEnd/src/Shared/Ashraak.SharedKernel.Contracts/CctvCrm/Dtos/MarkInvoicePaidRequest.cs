namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/invoices/{id}/mark-paid request body.</summary>
public sealed record MarkInvoicePaidRequest(
    DateTime? PaidAt,
    uint RowVersion);
