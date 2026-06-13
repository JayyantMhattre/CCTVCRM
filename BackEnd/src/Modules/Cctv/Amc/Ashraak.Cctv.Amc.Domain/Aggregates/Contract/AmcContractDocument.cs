using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract;

/// <summary>AMC contract document metadata.</summary>
public sealed class AmcContractDocument : Entity<AmcContractDocumentId>
{
    private AmcContractDocument(AmcContractDocumentId id) : base(id) { }

    public AmcContractId ContractId { get; private set; }
    public AmcContractTermId? TermId { get; private set; }
    public Guid FileId { get; private set; }
    public ContractDocumentType DocumentType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static AmcContractDocument Create(
        AmcContractDocumentId id,
        AmcContractId contractId,
        AmcContractTermId? termId,
        Guid fileId,
        ContractDocumentType documentType,
        string title,
        Guid createdBy)
    {
        return new AmcContractDocument(id)
        {
            ContractId = contractId,
            TermId = termId,
            FileId = fileId,
            DocumentType = documentType,
            Title = title.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
