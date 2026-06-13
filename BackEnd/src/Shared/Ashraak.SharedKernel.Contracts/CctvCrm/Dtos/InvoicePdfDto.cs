namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>GET /cctv/invoices/{id}/pdf — file metadata for download redirect.</summary>
public sealed record InvoicePdfDto(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid FileId,
    string DownloadUrl);
