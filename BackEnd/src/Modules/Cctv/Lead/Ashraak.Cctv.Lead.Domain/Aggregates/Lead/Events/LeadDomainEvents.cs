using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Events;

public sealed record LeadCreatedDomainEvent(
    Guid LeadId,
    string LeadNumber,
    LeadSource Source,
    string ContactName,
    string Email) : DomainEvent;

public sealed record LeadStatusChangedDomainEvent(
    Guid LeadId,
    string LeadNumber,
    LeadStatus FromStatus,
    LeadStatus ToStatus,
    Guid ChangedBy) : DomainEvent;

public sealed record LeadConvertedDomainEvent(
    Guid LeadId,
    string LeadNumber,
    Guid CustomerId,
    Guid SiteId,
    Guid ContractId,
    Guid ConvertedBy) : DomainEvent;

public sealed record LeadLostDomainEvent(
    Guid LeadId,
    string LeadNumber,
    Guid ChangedBy) : DomainEvent;
