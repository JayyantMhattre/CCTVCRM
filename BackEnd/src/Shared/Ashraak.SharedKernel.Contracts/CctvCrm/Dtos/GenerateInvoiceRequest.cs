namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/invoices/{id}/generate request body.</summary>
public sealed record GenerateInvoiceRequest(uint RowVersion);
