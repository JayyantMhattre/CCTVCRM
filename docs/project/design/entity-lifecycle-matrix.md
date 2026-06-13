# Entity Lifecycle Matrix

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-4 — Entity Model, ER Diagram & Database Architecture (design only)
**Source of truth:** [requirements-freeze-v1.md](../requirements-freeze-v1.md) · [business-rules.md](../business-rules.md)

For every major entity: who/what may **Create**, **Update**, **Delete** (soft), **Archive**, whether an **Approval** gate applies, and the approved **status transitions**. "System" = automated behavior mandated by the freeze document. ❌ = not permitted in V1.

---

## 1. Lead domain

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| Lead | **System** (website inquiry, BR-LEAD-02) · Admin (manual) | Admin (details, status) | Admin (erroneous/spam records only) | Terminal statuses (Lost, Converted) retained as pipeline history | ❌ none | `New → Contacted → Qualified → QuotationSent → Negotiation → Won → Converted`; `Negotiation → Lost`; conversion creates Customer + Site + Initial AMC Contract (BR-LEAD-03) |
| LeadActivity | System (on transitions) · Admin (manual log) | ❌ append-only | ❌ | With parent lead | ❌ | — |
| LeadRemark | Admin | ❌ append-only | ❌ | With parent lead | ❌ | — |
| LeadAttachment | Admin | ❌ (replace = new row) | Admin (while lead open) | With parent lead | ❌ | — |

## 2. Customer domain

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| Customer | Lead conversion (System) · Admin | Admin · **Customer (own profile only**, BR-AUTH-05) | Admin (erroneous records; blocked if contracts exist) | Deactivation (`Active → Inactive`) | ❌ | `Active ⇄ Inactive` |
| Site | Lead conversion (System) · Admin | Admin | Admin (blocked if active contract exists, BR-AMC-02) | Deactivation | ❌ | `Active ⇄ Inactive` |
| SiteContact | Admin (≤3 per site, BR-STRUCT-03) | Admin | Admin (slot freed for reuse) | With parent site | ❌ | — |
| SiteDocument | Admin | ❌ (replace = new row) | Admin | With parent site | ❌ | — |
| SiteAssetSummary | Admin (with site creation) | Admin (counts, brand/model/remarks — summary only, BR-STRUCT-04) | ❌ (1:1 with site) | With parent site | ❌ | — |

## 3. AMC domain

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| AMCPlan | Admin only (BR-VISIT-07) | Admin (identity fields; commercial changes = **new version**) | Admin (only if never referenced) | Retirement (`Active → Retired`; existing contracts unaffected) | ❌ | `Active → Retired` |
| AMCPlanVersion | Admin (new version per change, BR-AMC-06) | ❌ **immutable once referenced** (BR-AMC-07) | ❌ | Superseded by newer version (history kept) | ❌ | `Draft → Published → Superseded` |
| AMCContract | Lead conversion (System, initial) · Admin | Admin only (BR-VISIT-07) | ❌ **never** (permanent master, freeze §8) | Status terminal (Expired/Cancelled); record retained forever | ❌ | `Active → Expired` (last term ends) · `Active → Cancelled` (admin) |
| AMCContractTerm | Admin (new term = renewal; customer may **request**, BR-AMC-08) | Admin (Draft only; pinned plan version never changes) | ❌ never (renewal history, BR-AMC-01) | Expired terms retained as history (admin-visible, BR-AMC-04) | ❌ (admin-managed) | `Draft → Active → Expired`; `Draft/Active → Cancelled` |
| AMCContractDocument | System (generated Contract PDF, freeze §19) · Admin (uploads) | ❌ immutable once issued | ❌ | With parent contract | ❌ | — |

## 4. Service domain

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| ServiceSchedule | **System** (auto-generated from AMC frequency, BR-SCHED-02) · Admin (ad-hoc within term) | Admin (reschedule, BR-SCHED-03) | ❌ (use Cancelled status) | Terminal statuses retained | ❌ | `Planned → Assigned → InProgress → Completed`; `Planned/Assigned → Missed`; `Planned/Assigned → Cancelled` (BR-SCHED-01). Engineer assignment **mandatory** before InProgress (BR-SCHED-04) |
| EngineerAssignment | Admin | ❌ (reassignment = new row, old row inactivated) | ❌ | Inactive rows = assignment history | ❌ | `is_active: true → false` |
| ServiceVisit | Engineer (starts visit on assigned schedule) | Engineer (until report submitted); ❌ after approval | ❌ never (service record) | Retained permanently | ✅ **Admin approval required** before customer visibility (BR-VISIT-04/05) | `Started → EvidenceCaptured → Submitted → Approved`; `Submitted → Returned → Submitted` (re-review). Completion requires selfie + GPS + ≥1 photo + signature + remarks (BR-VISIT-01) |
| VisitPhoto | Engineer (Before/During/After/Selfie; offline-capable) | ❌ | Engineer (before submission only) | With parent visit | Via parent report approval | — |
| VisitLocation | Engineer (System-captured GPS: lat/long/timestamp, BR-VISIT-02) | ❌ immutable | ❌ | With parent visit | Via parent | — |
| VisitSignature | Engineer (customer signs on device) | ❌ immutable | ❌ | With parent visit | Via parent | — |
| VisitApproval | System (on report submission → Pending) | Admin decides (approve/return) | ❌ append-only | History of review rounds retained | — (this **is** the approval record) | `Pending → Approved` · `Pending → Returned` |
| VisitAttachment | Engineer (videos, reports, BR-VISIT-06) · System (Visit Report PDF, freeze §19) | ❌ | Engineer (before submission only) | With parent visit | Via parent | — |

## 5. Ticket domain

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| Ticket | **Customer · Admin · Engineer** (during visit) (BR-TKT-03..05) | Admin (any field) · Engineer (progress on assigned) · Customer (own: add info, reopen) | Admin (erroneous only) | Closed tickets retained; **customer may reopen** (BR-TKT-06) | ❌ | `Open → Assigned → InProgress → Resolved → Closed`; `Closed → Reopened → Assigned` (BR-TKT-01). Priority Low/Medium/High/Critical set at creation, admin-adjustable (BR-TKT-02) |
| TicketComment | Customer · Admin · Engineer (on accessible tickets) | ❌ append-only | ❌ | With parent ticket | ❌ | — |
| TicketAttachment | Customer · Admin · Engineer | ❌ | Author (while ticket open) | With parent ticket | ❌ | — |
| TicketAssignment | Admin | ❌ (reassignment = new row) | ❌ | Inactive rows = history | ❌ | `is_active: true → false` |
| TicketStatusHistory | **System** (every transition) | ❌ append-only | ❌ | Retained permanently | ❌ | — |

## 6. Engineer domain

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| Engineer | Admin | Admin | Admin (only if no assignment history) | Deactivation (`Active → Inactive`; history preserved) | ❌ | `Active ⇄ Inactive` |

## 7. Invoice domain (Option B)

| Entity | Create | Update | Delete (soft) | Archive | Approval | Status transitions |
|--------|--------|--------|---------------|---------|----------|--------------------|
| Invoice | Admin (types: AMCRenewal, NewAMC, EmergencyService, SpareReplacement, AdditionalCharges, Other) | Admin (**Draft only**; after Generated only status moves) | ❌ (use Cancelled status, BR-INV-01) | Terminal statuses (Paid/Cancelled) retained | ❌ (admin-managed lifecycle) | `Draft → Generated → Sent → Paid`; `Draft/Generated/Sent → Cancelled` (BR-INV-01). AMC-type invoices linked to a contract term (BR-INV-02) |
| InvoiceLine | Admin (Draft invoice only) | Admin (Draft only) | Admin (Draft only) | Locked with parent after Draft | ❌ | — |
| InvoiceAttachment | **System** (Invoice PDF on Generated, BR-INV-04/freeze §19) · Admin | ❌ immutable | ❌ | With parent invoice | ❌ | — |
| InvoiceStatusHistory | **System** (every transition) | ❌ append-only | ❌ | Retained permanently | ❌ | — |

---

## Cross-cutting lifecycle rules

| Rule | Applies to |
|------|-----------|
| Soft delete only; hard delete forbidden ([naming standards §7](./database-naming-standards.md)) | All business tables |
| Permanent records — no deletion of any kind | AMCContract, AMCContractTerm, AMCPlanVersion, ServiceVisit (+ evidence), all status histories |
| Every status transition emits a domain event → platform Audit + notifications where approved (freeze §17) | All lifecycle entities |
| Approval gates in V1 | **Visit reports only** (BR-VISIT-04) — no other entity has an approval workflow |
| Status vocabularies are frozen; changes require a change request (freeze §22) | All lifecycle entities |

---

## Related documents

- [entity-model.md](./entity-model.md)
- [business-rules.md](../business-rules.md)
- [workflow-overview.md](../workflow-overview.md)
