namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC contract document metadata.</summary>
public sealed record AmcContractDocumentDto(
    Guid Id,
    Guid ContractId,
    Guid? TermId,
    Guid FileId,
    string DocumentType,
    string Title,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
