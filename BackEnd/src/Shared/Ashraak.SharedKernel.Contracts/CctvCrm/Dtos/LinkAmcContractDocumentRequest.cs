namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/contracts/{contractId}/documents.</summary>
public sealed record LinkAmcContractDocumentRequest(
    Guid FileId,
    string DocumentType,
    string Title,
    Guid? TermId);
