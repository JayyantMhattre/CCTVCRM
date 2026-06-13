# ERD — Service Domain

**Schema:** `cctv_service` · **Modules:** Service Scheduling (8), Visit Management (9)
**Source of truth:** [requirements-freeze-v1.md §11–§13](../requirements-freeze-v1.md) · Rules: BR-SCHED-01..04, BR-VISIT-01..07

---

## ER diagram

```mermaid
erDiagram
    ServiceSchedule ||--o{ EngineerAssignment : "mandatory (BR-SCHED-04)"
    ServiceSchedule ||--o| ServiceVisit : "executed as"
    ServiceVisit ||--o{ VisitPhoto : "min 1 + selfie (BR-VISIT-01)"
    ServiceVisit ||--o| VisitLocation : "GPS (BR-VISIT-02)"
    ServiceVisit ||--o| VisitSignature : "customer signs"
    ServiceVisit ||--o{ VisitApproval : "admin review rounds (BR-VISIT-04)"
    ServiceVisit ||--o{ VisitAttachment : "videos + report PDF"

    ServiceSchedule {
        uuid id PK
        string schedule_number UK "VS-YYYY-NNNN"
        uuid amc_contract_term_id "logical ref cctv_amc - generation source"
        uuid site_id "logical ref cctv_customer"
        date scheduled_date
        int sequence_in_term "visit N of frequency"
        string status "Planned | Assigned | InProgress | Completed | Missed | Cancelled"
        date rescheduled_from "nullable - reschedule trail (BR-SCHED-03)"
        boolean is_auto_generated "true for frequency-generated (BR-SCHED-02)"
        timestamptz created_at
        uuid created_by "system or admin"
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        bytea row_version
    }

    EngineerAssignment {
        uuid id PK
        uuid service_schedule_id FK
        uuid engineer_id "logical ref cctv_engineer"
        uuid assigned_by "admin user"
        timestamptz assigned_at
        boolean is_active "false when reassigned - history kept"
        timestamptz created_at
        uuid created_by
    }

    ServiceVisit {
        uuid id PK
        uuid service_schedule_id FK "unique - one visit per schedule"
        uuid engineer_id "logical ref - executing engineer"
        timestamptz started_at
        timestamptz completed_at "nullable until evidence complete"
        string visit_remarks "MANDATORY before completion"
        string report_status "Draft | Submitted | Returned | Approved"
        boolean is_customer_visible "true only after approval (BR-VISIT-05)"
        timestamptz created_at
        uuid created_by
        timestamptz updated_at "nullable"
        uuid updated_by "nullable"
        bytea row_version
    }

    VisitPhoto {
        uuid id PK
        uuid service_visit_id FK
        uuid file_id "platform FileRecord - NO path columns"
        string category "Before | During | After | Selfie (BR-VISIT-03)"
        string caption "nullable"
        timestamptz captured_at
        timestamptz created_at
        uuid created_by "engineer"
    }

    VisitLocation {
        uuid id PK
        uuid service_visit_id FK "unique - 1:1"
        numeric latitude "numeric(9,6)"
        numeric longitude "numeric(9,6)"
        timestamptz captured_at "timestamp stored (BR-VISIT-02)"
        timestamptz created_at
        uuid created_by
    }

    VisitSignature {
        uuid id PK
        uuid service_visit_id FK "unique - 1:1"
        uuid file_id "signature image - platform FileRecord"
        string signed_by_name "site contact who signed"
        timestamptz captured_at
        timestamptz created_at
        uuid created_by
    }

    VisitApproval {
        uuid id PK
        uuid service_visit_id FK
        string decision "Pending | Approved | Returned"
        uuid reviewed_by "nullable until decided - admin"
        timestamptz reviewed_at "nullable"
        string review_remarks "nullable"
        timestamptz created_at
        uuid created_by
    }

    VisitAttachment {
        uuid id PK
        uuid service_visit_id FK
        uuid file_id "platform FileRecord"
        string attachment_type "Video | ReportPdf | Other (BR-VISIT-06)"
        string title "nullable"
        timestamptz created_at
        uuid created_by
    }
```

## Relationships

| Relationship | Cardinality | Type |
|--------------|-------------|------|
| ServiceSchedule → EngineerAssignment | 1:N (one active) | Composition; reassignment keeps history |
| ServiceSchedule → ServiceVisit | 1:0..1 | Composition; unique schedule_id |
| ServiceVisit → VisitPhoto / VisitAttachment / VisitApproval | 1:N | Composition |
| ServiceVisit → VisitLocation / VisitSignature | 1:1 | Composition; unique visit_id |
| ServiceSchedule → AMCContractTerm / Site | N:1 | **Logical** cross-schema |
| EngineerAssignment / ServiceVisit → Engineer | N:1 | **Logical** cross-schema |
| Media → FileRecord | N:1 | **Logical** platform reference (`file_id`) — no path columns |

## Constraints & indexes

| Object | Definition |
|--------|-----------|
| `ux_service_visits_service_schedule_id` | one visit per schedule |
| `ux_visit_locations_service_visit_id`, `ux_visit_signatures_service_visit_id` | 1:1 evidence |
| `ux_engineer_assignments_schedule_active` | unique (service_schedule_id) WHERE is_active — one active engineer |
| `ck_service_schedules_status` | frozen status list (BR-SCHED-01) |
| `ck_visit_photos_category` | Before/During/After/Selfie |
| `ck_visit_locations_range` | lat ∈ [-90,90], lng ∈ [-180,180] |
| `ix_service_schedules_scheduled_date`, `ix_service_schedules_status` | calendar/queue queries |
| `ix_engineer_assignments_engineer_id` | engineer work queue (freeze §2) |

### Completion invariant (BR-VISIT-01) — application-enforced at the aggregate boundary

A `ServiceVisit` may transition to completed/Submitted **only when all hold**:
`∃ VisitPhoto(category=Selfie)` · `∃ VisitPhoto(category∈Before/During/After)` · `∃ VisitLocation` · `∃ VisitSignature` · `visit_remarks` non-empty.

### Offline capture (freeze §18)

Engineer app captures evidence offline; on sync it uploads files (→ FileIds) then submits the visit aggregate. `captured_at` reflects capture time, not sync time.

## Domain events

| Event | Notes |
|-------|-------|
| SchedulesGenerated (per term) | from TermActivated (BR-SCHED-02); audit |
| VisitScheduled / Rescheduled / Cancelled / Missed | Notification "Visit Scheduled" (freeze §17); audit |
| EngineerAssigned / Reassigned | audit |
| VisitStarted / ReportSubmitted | audit |
| VisitReportApproved / Returned | approval gate (BR-VISIT-04); customer visibility flips on approval; audit |
| VisitCompleted | Notification "Visit Completed" (freeze §17); audit |
| VisitReportPdfGenerated | attachment row (freeze §19) |

Related: [entity-model.md §2.4](./entity-model.md) · [entity-lifecycle-matrix.md §4](./entity-lifecycle-matrix.md) · [workflow-overview.md §4](../workflow-overview.md)
