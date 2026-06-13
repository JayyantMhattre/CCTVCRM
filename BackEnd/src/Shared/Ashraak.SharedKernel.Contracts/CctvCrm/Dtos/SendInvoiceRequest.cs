namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/invoices/{id}/send request body.</summary>
public sealed record SendInvoiceRequest(uint RowVersion);
