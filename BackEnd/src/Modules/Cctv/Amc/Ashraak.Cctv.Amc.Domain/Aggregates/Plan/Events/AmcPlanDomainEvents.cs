using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Plan.Events;

public sealed record PlanCreatedDomainEvent(
    Guid PlanId,
    string PlanCode,
    string Name,
    Guid CreatedBy) : DomainEvent;

public sealed record PlanVersionPublishedDomainEvent(
    Guid PlanId,
    Guid VersionId,
    int VersionNo,
    Guid PublishedBy) : DomainEvent;

public sealed record PlanRetiredDomainEvent(
    Guid PlanId,
    string PlanCode,
    Guid RetiredBy) : DomainEvent;
