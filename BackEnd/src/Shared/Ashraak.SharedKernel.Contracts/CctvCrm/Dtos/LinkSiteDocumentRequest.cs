namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Request body for POST /cctv/sites/{siteId}/documents.</summary>
public sealed record LinkSiteDocumentRequest(
    Guid FileId,
    string DocumentType,
    string Title,
    uint RowVersion);
