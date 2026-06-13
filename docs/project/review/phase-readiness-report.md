# Phase Readiness Report

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-0 — Validate B1–B7 executable without ambiguity  
**Reference:** [phase-execution-playbook.md](../roadmap/phase-execution-playbook.md) · [backend-development-phases.md](../roadmap/backend-development-phases.md)

---

## 1. Summary

| Phase | Ready? | Ambiguity level | Can start after |
|-------|:------:|:---------------:|-----------------|
| D1 Bootstrap | ✅ | Low | D1-0 approval |
| B1 Lead | ✅ | Low | D1 exit |
| B2 Customer/Site | ✅ | Low | B1 exit |
| B3 AMC | ✅ | Low | B2 exit |
| B4 Scheduling/Visits | ✅ | Medium | B3 exit |
| B5 Tickets/Engineer | ✅ | Low | B2 exit (B4 for visit-link) |
| B6 Invoices/PDF | ✅ | Medium | B3 exit |
| B7 Reports | ✅ | Low | B1–B6 data exists |

**All phases B1–B7 can be executed without architectural ambiguity.** Medium items are **implementation choices with documented defaults** — not blockers.

---

## 2. Phase D1 — Bootstrap & Foundation

| Gate item | Status | Ambiguity |
|-----------|:------:|-----------|
| Module skeleton projects | Designed | None — follow add-backend-module.md |
| RBAC seed script | Designed | None — permission-catalog.md lists all 30 |
| Portal route skeleton | Designed | None — navigation-architecture.md |
| SMS interface stub | Designed | Provider TBD — ADR required |
| PDF interface stub | Designed | Library TBD — ADR required |
| `/cctv/health` smoke | Designed | None |
| Architecture tests | Designed | None |
| Module doc stubs | Required | Template in governance |

**Readiness: ✅ READY** — first implementation phase after this review.

---

## 3. Phase B1 — Lead Management

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| Entities (4) | ✅ | entity-model §2.1 |
| APIs (13 routes) | ✅ | endpoint-catalog §Lead |
| Events (4) | ✅ | event-catalog |
| Notifications | ✅ | Lead Created → admin |
| Conversion command | ⚠️ | **Partial until B3** — explicitly documented; mock/stub downstream in B1 tests |
| BR gates | ✅ | BR-LEAD-01..03 in playbook |
| Admin UI | ✅ | Screens #12–14, forms in form-catalog |
| Public inquiry | ✅ | Screens #8–9; rate limit REUSE |

**Ambiguities resolved:**
- Conversion creates customer+site in B2; contract in B3 — **playbook and module-contracts agree**
- Won vs Converted terminal states — lifecycle matrix clear

**Readiness: ✅ READY after D1**

---

## 4. Phase B2 — Customer · Site · Asset

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| Entities (5) | ✅ | Max 3 contacts as aggregate invariant |
| APIs | ✅ | customers, sites, asset-summary, portal/profile |
| Lead conversion consumer | ✅ | Implements ILeadConversionService |
| Asset strategy | ✅ | Summary counts only — no camera entities |
| BR gates | ✅ | BR-STRUCT-01..05 |
| Admin UI | ✅ | Screens #15–20 |

**Readiness: ✅ READY after B1**

---

## 5. Phase B3 — AMC Plans · Contracts · Terms

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| Entities (5) | ✅ | Master + Terms model |
| Plan versioning | ✅ | AMCPlanVersion immutable snapshot |
| One active contract/site | ✅ | DB + domain double enforcement |
| Renewal request | ✅ | Customer API + admin queue (screen gap M-02 — UI layout only) |
| Expiry job | ✅ | ExpiryReminderDue event |
| Contract PDF | ✅ | First PDF type — pdf-document-design §1 |
| BR gates | ✅ | BR-AMC-01..08 |
| Completes lead conversion | ✅ | B1 conversion fully closed here |

**Medium ambiguity:** Public AMC plans for website (M-03) — default: static marketing content; optional anonymous API ADR.

**Readiness: ✅ READY after B2**

---

## 6. Phase B4 — Scheduling · Visits · Approval

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| Entities (8) | ✅ | Largest domain — erd-service-domain |
| Auto schedule generation | ✅ | Handler on TermActivated |
| Visit evidence | ✅ | Selfie, GPS, photos, signature — validation-rules §6 |
| Approval workflow | ✅ | workflow-screen-design §4 |
| Visit report PDF | ✅ | Second PDF type |
| Engineer sync API | ✅ | mobile-api-consumption.md |
| BR gates | ✅ | BR-SCHED-01..04, BR-VISIT-01..05 |
| Admin UI | ✅ | Screens #26–29 |
| Engineer portal start | ✅ | Visit reporting partial in FP-8 |

**Medium ambiguity:** Schedule frequency edge cases (monthly/quarterly) — unit test matrix in testing-roadmap; no design gap.

**Readiness: ✅ READY after B3**

---

## 7. Phase B5 — Tickets · Engineer Operations

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| Ticket entities (5) | ✅ | Full lifecycle + history |
| Engineer entity (1) | ✅ | Links platform User |
| Tri-actor creation | ✅ | Customer, Admin, Engineer APIs |
| Reopen | ✅ | Customer action BR-TKT-06 |
| Assignment | ✅ | TicketAssignment entity |
| BR gates | ✅ | BR-TKT-01..06 |
| Admin UI | ✅ | Screens #30–34 |
| Can parallel B4 | ✅ | Tickets without visit link first |

**Readiness: ✅ READY after B2** (full visit-link after B4)

---

## 8. Phase B6 — Invoices · PDF Generation

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| Entities (4) | ✅ | Option B in entity-model |
| Invoice types | ✅ | 6 types in validation-rules |
| Term link rule | ✅ | Option B — V-INV-02/03 (not literal BR-INV-02) |
| Invoice PDF | ✅ | Third PDF type |
| Lifecycle | ✅ | Draft → Generated → Sent → Paid |
| No accounting | ✅ | BR-INV-05 enforced |
| BR gates | ✅ | BR-INV-01..05 with Option B interpretation |
| Admin UI | ✅ | Screens #35–37 |

**Medium ambiguity:** PDF library selection — D1 ADR + spike; storage via Files regardless.

**Readiness: ✅ READY after B3** (visit/ticket-linked invoices enrich after B4/B5)

---

## 9. Phase B7 — Reports · Analytics

| Aspect | Clarity | Notes |
|--------|:-------:|-------|
| No new schema | ✅ | Read-only queries |
| 6 admin reports | ✅ | report-specification.md |
| Dashboard widgets | ✅ | dashboard-specification.md |
| Tables not charts | ✅ | By design — no chart library |
| Export | ✅ | CSV per report-spec |
| APIs | ✅ | `/reports/*`, dashboard endpoints |

**Readiness: ✅ READY after B1–B6 data populated**

---

## 10. Cross-phase ambiguity register

| ID | Topic | Phases | Resolution |
|----|-------|--------|------------|
| A-01 | Lead conversion partial | B1–B3 | Documented; integration tests with mocks |
| A-02 | Option B invoicing | B6 | Server implements Option B; UAT script |
| A-03 | Admin renewal queue screen | B3/FP-3 | Embed in contracts or add screen #72 |
| A-04 | Public AMC plans | B3/website | Static content default |
| A-05 | SMS provider | D1–REL | ADR Sprint 0; email fallback dev |
| A-06 | Audit read stub | All | Business histories; accept V1 limitation |

**None require architecture redesign.**

---

## 11. Conclusion

Phases B1 through B7 have **complete inputs, outputs, gates, and traceability** in the D0 pack. Execution playbook gates are sufficient to prevent premature starts. **Proceed to D1 bootstrap, then B1 Lead Management.**

---

Related: [phase-execution-playbook.md](../roadmap/phase-execution-playbook.md) · [sprint-plan.md](../roadmap/sprint-plan.md)
