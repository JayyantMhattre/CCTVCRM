namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Site-linked document metadata.</summary>
public sealed record SiteDocumentDto(
    Guid Id,
    Guid FileId,
    string DocumentType,
    string Title,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
