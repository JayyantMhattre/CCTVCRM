# Workflow Screen Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7 — screen-by-screen workflow specifications
**Flows:** [user-journeys.md](../user-journeys.md) · [workflow-overview.md](../../workflow-overview.md)

Each workflow lists screens in order, user actions, validations, and success/error UX.

---

## 1. Lead Conversion

| Step | Screen | Actor | Action | Validation | Success UX | Error UX |
|------|--------|-------|--------|------------|------------|----------|
| 1 | Lead List #12 | Admin | Open Won lead | — | Navigate detail | — |
| 2 | Lead Detail #13 | Admin | Click Convert | status=Won | Open wizard #14 | Toast if not Won |
| 3 | Lead Conversion #14 | Admin | Enter site + plan version + dates | V-AMC-08, V-SITE-* | Progress stepper | Field errors |
| 4 | Lead Conversion #14 | Admin | Confirm convert | V-LEAD-02 | Toast success → Customer #16 | ProblemDetails toast |
| 5 | Customer Detail #16 | Admin | Review created entities | — | Shows site + contract links | — |

**Post-actions:** Email to customer (welcome) · Admin notification · Audit event

---

## 2. AMC Renewal

| Step | Screen | Actor | Action | Validation | Success UX |
|------|--------|-------|--------|------------|------------|
| 1 | Customer Dashboard #46 / AMC #47 | Customer | See expiry CTA | days to expiry | Renewal button visible |
| 2 | Request Renewal #48 | Customer | Submit optional message | V-AMC-07 | Toast "Request submitted" |
| 3 | Admin Contract List #23 / Renewal queue | Admin | Open renewal request | — | Navigate contract |
| 4 | Term Create/Renew #25 | Admin | Create renewal term + activate | V-AMC-04 | Toast + schedules generated |
| 5 | Customer AMC #47 | Customer | See new active term only | BR-AMC-03 | Updated validity dates |

**Optional:** Invoice AmcRenewal flow (workflow 6)

---

## 3. Schedule Visit (assign engineer)

| Step | Screen | Actor | Action | Validation | Success UX |
|------|--------|-------|--------|------------|------------|
| 1 | Schedule List #26 | Admin | Filter Planned schedules | — | List filtered |
| 2 | Schedule Detail #27 | Admin | Select engineer + Assign | V-SCHED-02 | Status → Assigned |
| 3 | — | System | Notifications | — | Email/SMS/push (§17) |
| 4 | Engineer Home #59 / Visits #60 | Engineer | See assigned visit | scoped | Appears in My Day |
| 5 | Customer Upcoming #49 | Customer | See scheduled visit | own site | Date displayed |

**Alternate:** Admin reschedule (#27) with reason · Cancel with reason

---

## 4. Complete Visit (Engineer → Admin → Customer)

| Step | Screen | Actor | Action | Validation | Success UX |
|------|--------|-------|--------|------------|------------|
| 1 | Visit Detail #61 | Engineer | Start visit | assigned | Status InProgress |
| 2 | Visit Reporting #62 | Engineer | Capture selfie | V-VISIT-01 | Checklist ✓ |
| 3 | #62 | Engineer | Capture GPS | V-VISIT-02 | Checklist ✓ |
| 4 | #62 | Engineer | Upload photos (≥1) | V-VISIT-03 | Checklist ✓ |
| 5 | #62 | Engineer | Capture signature | V-VISIT-04 | Checklist ✓ |
| 6 | #62 | Engineer | Enter remarks | V-VISIT-05 | Checklist ✓ |
| 7 | #62 | Engineer | Submit report | all V-VISIT | Toast "Submitted for review" |
| 8 | Approval Queue #28 | Admin | Open pending visit | — | Review #29 |
| 9 | Review Detail #29 | Admin | Review evidence | — | Gallery view |
| 10a | #29 | Admin | Approve | — | Toast; customer notified |
| 10b | #29 | Admin | Return | V-VISIT-08 | Engineer sees in Returned queue |
| 11 | Service History #50 | Customer | View report + PDF | approved only | Download enabled |

**Mobile offline:** Steps 2–7 queued locally; sync on reconnect (workflow unchanged, sync screen #71)

---

## 5. Create Ticket

| Step | Screen | Actor | Action | Validation | Success UX |
|------|--------|-------|--------|------------|------------|
| 1 | Ticket Create #32/#52/#69 | Actor | Fill form + attachments | V-TKT-* | — |
| 2 | Same | Actor | Submit | site scope | Toast + ticket number |
| 3 | Ticket Detail #31/#53/#68 | Actor | View created ticket | — | Status Open |
| 4 | Ticket List #30 | Admin | Assign engineer | — | Status Assigned + notifications |
| 5 | Ticket Detail #68 | Engineer | Progress to In Progress | assigned | Status update |
| 6 | Ticket Detail #68 | Engineer | Resolve | — | Status Resolved |
| 7 | Ticket Detail #31 | Admin | Close | V-TKT-06 | Status Closed + notification |
| 8 | Ticket Detail #53 | Customer | Reopen (optional) | V-TKT-04/05 | Back to workflow step 4 |

---

## 6. Generate Invoice (Option B)

| Step | Screen | Actor | Action | Validation | Success UX |
|------|--------|-------|--------|------------|------------|
| 1 | Invoice Create #36 | Admin | Select type + customer | V-INV-02/03 | — |
| 2 | #36 | Admin | Add line items | V-INV-05/06 | Running total preview |
| 3 | #36 | Admin | Save draft | V-INV-04 | Navigate detail #37 |
| 4 | Invoice Detail #37 | Admin | Generate | V-INV-07 | PDF created + status Generated |
| 5 | #37 | Admin | Send | — | Status Sent + notification |
| 6 | Invoice Detail #55 | Customer | Download PDF | V-INV-08 | File download |
| 7 | Invoice Detail #37 | Admin | Mark paid (manual) | — | Status Paid |

**Cancel path:** Draft/Generated/Sent → Cancelled with reason dialog

---

## Workflow UI patterns (REUSE)

| Pattern | Component |
|---------|-----------|
| Multi-step wizard | `PlatformCard` steps + Next/Back |
| Confirm destructive | `PlatformConfirmDialog` |
| Status change | Inline select + confirm or modal |
| Success feedback | `toast.success()` |
| Blocking validation | Disable primary button + checklist (visit) |
| Optimistic lock conflict | Toast + refetch on 409 |

---

Related: [screen-design-specification.md](./screen-design-specification.md) · [validation-rules.md](./validation-rules.md) · [notification-ux-design.md](./notification-ux-design.md)
