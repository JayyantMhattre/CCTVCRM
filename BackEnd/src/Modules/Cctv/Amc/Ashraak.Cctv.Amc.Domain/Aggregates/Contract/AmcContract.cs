using Ashraak.Cctv.Amc.Domain.Aggregates.Contract.Events;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract;

/// <summary>AMC contract aggregate root (schema <c>cctv_amc.amc_contracts</c>).</summary>
public sealed class AmcContract : AggregateRoot<AmcContractId>
{
    private readonly List<AmcContractTerm> _terms = [];
    private readonly List<AmcContractDocument> _documents = [];

    private AmcContract(AmcContractId id) : base(id) { }

    public string ContractNumber { get; private set; } = string.Empty;
    public Guid SiteId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? SourceLeadId { get; private set; }
    public ContractStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public uint RowVersion { get; private set; }

    public IReadOnlyList<AmcContractTerm> Terms => _terms.AsReadOnly();
    public IReadOnlyList<AmcContractDocument> Documents => _documents.AsReadOnly();

    public static AmcContract Create(
        AmcContractId id,
        string contractNumber,
        Guid siteId,
        Guid customerId,
        Guid? sourceLeadId,
        Guid createdBy)
    {
        var contract = new AmcContract(id)
        {
            ContractNumber = contractNumber,
            SiteId = siteId,
            CustomerId = customerId,
            SourceLeadId = sourceLeadId,
            Status = ContractStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        contract.RaiseDomainEvent(new ContractCreatedDomainEvent(
            id.Value, contractNumber, siteId, customerId, sourceLeadId, createdBy));

        return contract;
    }

    public AmcContractTerm AddTerm(
        AmcContractTermId termId,
        AmcPlanVersionId planVersionId,
        DateOnly startDate,
        DateOnly endDate,
        decimal agreedPrice,
        TermOrigin origin,
        Guid createdBy)
    {
        EnsureNotCancelled();

        var termNo = _terms.Count == 0 ? 1 : _terms.Max(t => t.TermNo) + 1;
        var term = AmcContractTerm.Create(
            termId,
            Id,
            termNo,
            planVersionId,
            startDate,
            endDate,
            agreedPrice,
            origin,
            createdBy);

        _terms.Add(term);
        Touch(createdBy);

        RaiseDomainEvent(new TermCreatedDomainEvent(
            Id.Value, termId.Value, termNo, origin.ToString(), createdBy));

        return term;
    }

    public void ActivateTerm(AmcContractTermId termId, Guid activatedBy)
    {
        EnsureNotCancelled();

        var term = GetTerm(termId);

        foreach (var active in _terms.Where(t => t.Status == TermStatus.Active))
            active.DeactivateToExpired(activatedBy);

        term.Activate(activatedBy);
        Status = ContractStatus.Active;
        Touch(activatedBy);

        RaiseDomainEvent(new TermActivatedDomainEvent(
            Id.Value, termId.Value, term.TermNo, activatedBy));
    }

    public void Cancel(Guid cancelledBy)
    {
        if (Status == ContractStatus.Cancelled)
            return;

        Status = ContractStatus.Cancelled;

        foreach (var term in _terms.Where(t => t.Status is TermStatus.Draft or TermStatus.Active))
            term.Cancel(cancelledBy);

        Touch(cancelledBy);
    }

    public void RequestRenewal(AmcContractTermId termId, Guid requestedBy)
    {
        var term = GetActiveTerm()
            ?? throw new InvalidOperationException("No active term found for renewal request.");

        if (term.Id != termId)
            throw new InvalidOperationException("Renewal can only be requested for the active term.");

        term.RequestRenewal(requestedBy);
        Touch(requestedBy);

        RaiseDomainEvent(new RenewalRequestedDomainEvent(
            Id.Value, termId.Value, CustomerId, term.RenewalRequestedAtUtc!.Value));
    }

    public AmcContractDocument LinkDocument(
        AmcContractDocumentId documentId,
        Guid fileId,
        ContractDocumentType documentType,
        string title,
        AmcContractTermId? termId,
        Guid createdBy)
    {
        var document = AmcContractDocument.Create(
            documentId, Id, termId, fileId, documentType, title, createdBy);

        _documents.Add(document);
        Touch(createdBy);

        if (documentType == ContractDocumentType.ContractPdf)
        {
            RaiseDomainEvent(new ContractPdfGeneratedDomainEvent(
                Id.Value, documentId.Value, fileId, createdBy));
        }

        return document;
    }

    public AmcContractTerm GetTerm(AmcContractTermId termId) =>
        _terms.FirstOrDefault(t => t.Id == termId)
        ?? throw new InvalidOperationException("Contract term not found.");

    public AmcContractTerm? GetActiveTerm() =>
        _terms.FirstOrDefault(t => t.Status == TermStatus.Active);

    private void EnsureNotCancelled()
    {
        if (Status == ContractStatus.Cancelled)
            throw new InvalidOperationException("Cancelled contracts are read-only.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }
}
