namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/tickets/{id}/attachments request body.</summary>
public sealed record LinkTicketAttachmentRequest(
    Guid FileId,
    string? Title);
