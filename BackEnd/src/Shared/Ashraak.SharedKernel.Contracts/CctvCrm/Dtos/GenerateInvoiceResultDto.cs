namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/invoices/{id}/generate response.</summary>
public sealed record GenerateInvoiceResultDto(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid PdfFileId);
