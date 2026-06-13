namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Attachment metadata row.</summary>
public sealed record LeadAttachmentDto(
    Guid Id,
    Guid LeadId,
    Guid FileId,
    string Title,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
