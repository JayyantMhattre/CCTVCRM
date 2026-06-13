namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Link a platform file to a lead (POST /cctv/leads/{id}/attachments).</summary>
public sealed record LinkLeadAttachmentRequest(Guid FileId, string Title);
