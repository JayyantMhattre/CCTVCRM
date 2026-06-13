# Service module — domain events

Schema: `cctv_service` · Raised from aggregates, serialized to outbox via `BaseDbContext`.

## Schedule events

| Event | Trigger |
|-------|---------|
| `SchedulesGeneratedDomainEvent` | Bulk generation after term activation |
| `VisitScheduledDomainEvent` | Schedule created (auto or ad-hoc) |
| `VisitRescheduledDomainEvent` | Admin reschedule |
| `VisitCancelledDomainEvent` | Admin cancel |
| `VisitMissedDomainEvent` | Overdue / missed detection |
| `EngineerAssignedDomainEvent` | First assignment |
| `EngineerReassignedDomainEvent` | Reassignment (history kept) |

## Visit events

| Event | Trigger |
|-------|---------|
| `VisitStartedDomainEvent` | Engineer starts visit |
| `VisitReportSubmittedDomainEvent` | Report submitted for approval |
| `VisitReportApprovedDomainEvent` | Admin approves |
| `VisitReportReturnedDomainEvent` | Admin returns for rework |
| `VisitCompletedDomainEvent` | Approval completes visit (customer visibility) |

## Cross-module integration

| Source | Handler | Action |
|--------|---------|--------|
| `TermActivatedDomainEvent` (AMC) | `TermActivatedScheduleGenerationHandler` | Calls `IScheduleGenerationService.GenerateSchedulesForTermAsync` |

Downstream notifications (Visit Scheduled, Visit Completed) deferred to integration phase.
