using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract.Events;

public sealed record ContractCreatedDomainEvent(
    Guid ContractId,
    string ContractNumber,
    Guid SiteId,
    Guid CustomerId,
    Guid? SourceLeadId,
    Guid CreatedBy) : DomainEvent;

public sealed record TermCreatedDomainEvent(
    Guid ContractId,
    Guid TermId,
    int TermNo,
    string Origin,
    Guid CreatedBy) : DomainEvent;

public sealed record TermActivatedDomainEvent(
    Guid ContractId,
    Guid TermId,
    int TermNo,
    Guid ActivatedBy) : DomainEvent;

public sealed record TermExpiredDomainEvent(
    Guid ContractId,
    Guid TermId,
    int TermNo) : DomainEvent;

public sealed record RenewalRequestedDomainEvent(
    Guid ContractId,
    Guid TermId,
    Guid CustomerId,
    DateTime RequestedAtUtc) : DomainEvent;

public sealed record ContractPdfGeneratedDomainEvent(
    Guid ContractId,
    Guid DocumentId,
    Guid FileId,
    Guid CreatedBy) : DomainEvent;
