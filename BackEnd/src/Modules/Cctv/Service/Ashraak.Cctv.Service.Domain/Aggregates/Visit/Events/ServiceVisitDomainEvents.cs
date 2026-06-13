using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit.Events;

public sealed record VisitStartedDomainEvent(
    Guid VisitId,
    Guid ScheduleId,
    Guid EngineerId,
    DateTime StartedAtUtc) : DomainEvent;

public sealed record VisitReportSubmittedDomainEvent(
    Guid VisitId,
    Guid ScheduleId,
    Guid EngineerId,
    Guid SubmittedBy) : DomainEvent;

public sealed record VisitReportApprovedDomainEvent(
    Guid VisitId,
    Guid ScheduleId,
    Guid ReviewedBy) : DomainEvent;

public sealed record VisitReportReturnedDomainEvent(
    Guid VisitId,
    Guid ScheduleId,
    Guid ReviewedBy,
    string ReturnReason) : DomainEvent;

public sealed record VisitCompletedDomainEvent(
    Guid VisitId,
    Guid ScheduleId,
    Guid SiteId) : DomainEvent;
