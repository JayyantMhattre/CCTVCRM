namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/attachments.</summary>
public sealed record LinkVisitAttachmentRequest(
    Guid FileId,
    string AttachmentType,
    string? Title);
