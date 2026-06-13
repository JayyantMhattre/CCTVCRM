using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Service.Domain.Aggregates.Schedule.Events;

public sealed record SchedulesGeneratedDomainEvent(
    Guid ContractId,
    Guid TermId,
    int Count,
    Guid CreatedBy) : DomainEvent;

public sealed record VisitScheduledDomainEvent(
    Guid ScheduleId,
    string ScheduleNumber,
    Guid SiteId,
    DateOnly ScheduledDate,
    Guid CreatedBy) : DomainEvent;

public sealed record VisitRescheduledDomainEvent(
    Guid ScheduleId,
    string ScheduleNumber,
    DateOnly PreviousDate,
    DateOnly NewDate,
    Guid UpdatedBy) : DomainEvent;

public sealed record VisitCancelledDomainEvent(
    Guid ScheduleId,
    string ScheduleNumber,
    string Reason,
    Guid UpdatedBy) : DomainEvent;

public sealed record VisitMissedDomainEvent(
    Guid ScheduleId,
    string ScheduleNumber,
    Guid UpdatedBy) : DomainEvent;

public sealed record EngineerAssignedDomainEvent(
    Guid ScheduleId,
    Guid EngineerId,
    Guid AssignedBy) : DomainEvent;

public sealed record EngineerReassignedDomainEvent(
    Guid ScheduleId,
    Guid PreviousEngineerId,
    Guid NewEngineerId,
    Guid AssignedBy) : DomainEvent;
