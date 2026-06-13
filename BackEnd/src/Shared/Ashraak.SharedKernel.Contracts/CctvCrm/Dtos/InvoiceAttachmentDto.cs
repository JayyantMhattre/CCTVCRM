namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Invoice attachment metadata.</summary>
public sealed record InvoiceAttachmentDto(
    Guid Id,
    Guid FileId,
    string AttachmentType,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
