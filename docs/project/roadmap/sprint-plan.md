# Sprint Plan

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Sprint length:** 2 weeks (recommended)  
**Team assumption:** 2–3 full-stack devs + QA from Sprint 8

Adjusted from requested 10-sprint sequence based on [implementation-roadmap.md](./implementation-roadmap.md) dependency analysis.

---

## Sprint overview

| Sprint | Focus | Backend | Frontend | Mobile | Gate |
|:------:|-------|---------|----------|--------|------|
| **0** | Bootstrap & foundation | D1 | FP-0 shell | M0 SDK prep | D1 playbook |
| **1** | Lead + public inquiry | B1 | FP-1 admin lead + website forms | — | B1 |
| **2** | Customer + Site + Asset | B2 | FP-2 admin customer/site | — | B2 |
| **3** | AMC | B3 | FP-3 admin AMC | — | B3 |
| **4** | Scheduling + Visits | B4 (+ engineer stub) | FP-4 admin schedules/visits | — | B4 |
| **5** | Tickets + Engineers | B5 | FP-5 admin tickets/engineers | — | B5 |
| **6** | Invoices + PDFs | B6 (+ PDF service complete) | FP-6 admin invoices | — | B6 |
| **7** | Customer Portal | API polish | FP-7 customer portal | — | FP-C |
| **8** | Engineer Portal | Sync API hardening | FP-8 engineer portal | M0 complete | FP-E |
| **9** | Mobile Apps | Bugfixes | — | M1 + M2 | M playbook |
| **10** | Reports + Hardening | B7 | FP-9 reports/dashboard | M3 release prep | B7 |
| **11** | QA/UAT buffer | Fixes | Fixes | Beta fixes | REL |

**Total:** 11 sprints (22 weeks) + 2 weeks UAT ≈ **6 months calendar**

---

## Sprint 0 — Project bootstrap review & foundation

**Goal:** Ready to build B1 without rework

| Work item | Owner |
|-----------|-------|
| Review D0 design pack sign-off | All |
| Create CCTV module projects + slnx | Backend |
| RBAC seed migration/script | Backend |
| Portal routes + nav config skeleton | Frontend |
| SMS + PDF interface + ADR | Backend |
| Architecture tests | Backend |
| OpenAPI export in CI | DevOps |
| Stubs `docs/modules/cctv-*` | All |

**Demo:** Login as Admin/Engineer/Customer roles; empty CCTV health endpoint

---

## Sprint 1 — Lead Management

**Goal:** Inquiry → lead → pipeline management

| Backend | Frontend |
|---------|----------|
| B1 full module | Lead list/detail (#12–13) |
| Inquiry API | Get Quote + Contact forms (#8–9) |
| Lead events + admin notification | Conversion wizard UI shell (#14) |

**Tests:** BR-LEAD-01..02 scenarios **written**; execution at Review Gate 2  
**Gate:** Review Gate 1 (build + architecture + docs + completion report)

---

## Sprint 2 — Customer + Site

**Goal:** Masters + lead conversion completion

| Backend | Frontend |
|---------|----------|
| B2 module | Customer CRUD (#15–17) |
| Conversion orchestration wired | Site CRUD + asset (#18–20) |
| Portal profile API | Complete conversion wizard |

**Tests:** BR-STRUCT-01..05; max 3 contacts  
**Milestone:** M2 Lead-to-customer path

---

## Sprint 3 — AMC

**Goal:** Plans, contracts, terms, renewal

| Backend | Frontend |
|---------|----------|
| B3 module | Plan + contract UI (#21–25) |
| Expiry reminder job | Renewal request queue |
| Term activation event | |

**Tests:** BR-AMC-01..08; one active contract  
**Milestone:** M3 AMC live

---

## Sprint 4 — Scheduling + Visits

**Goal:** Field operations core

| Backend | Frontend |
|---------|----------|
| B4 module | Schedule UI (#26–27) |
| Schedule auto-generation handler | Approval queue (#28–29) |
| Visit execution + approval APIs | Engineer portal visit reporting **start** |

**Tests:** BR-VISIT-01 server-side; approval gate  
**Milestone:** M4 Field operations  
**Parallel:** Begin engineer master if needed for assignment

---

## Sprint 5 — Tickets + Engineers

**Goal:** Service desk

| Backend | Frontend |
|---------|----------|
| B5 modules | Ticket admin UI (#30–32) |
| Engineer master | Engineer admin (#33–34) |
| Ticket notifications | Engineer ticket queue (portal partial) |

**Tests:** BR-TKT-01..06  
**Milestone:** M5 Service desk

---

## Sprint 6 — Invoices + PDFs

**Goal:** Billing + all PDF types

| Backend | Frontend |
|---------|----------|
| B6 module | Invoice UI (#35–37) |
| PDF service (contract, visit, invoice) | PDF download actions |
| Invoice notifications | |

**Tests:** Option B; PDF generation smoke  
**Milestone:** M6 Billing

---

## Sprint 7 — Customer Portal

**Goal:** Customer self-service web complete

| Work |
|------|
| FP-7 all screens (#46–58) |
| `/portal/dashboard` integration |
| E2E: ticket create, renewal request, invoice PDF |
| API hardening for scoped reads |

**Prerequisite:** B2–B6 APIs stable  
**Milestone:** M7 partial (customer)

---

## Sprint 8 — Engineer Portal

**Goal:** Engineer web complete

| Work |
|------|
| FP-8 all screens (#59–70) |
| Visit reporting UX + checklist |
| Sync API finalization |
| E2E: full visit workflow web |

**Milestone:** M7 partial (engineer)

---

## Sprint 9 — Mobile Apps

**Goal:** Both apps per freeze §18

| Work |
|------|
| M1 Customer app (all tabs) |
| M2 Engineer app + **offline sync** |
| Regenerate Dart SDK |
| Device testing matrix |
| Beta build (fastlane) |

**Milestone:** M8 Mobile complete

---

## Sprint 10 — Reports + Hardening

**Goal:** V1 feature complete

| Work |
|------|
| B7 reporting module |
| FP-9 admin dashboard widgets + reports hub |
| Performance tuning |
| Full regression suite |
| Documentation finalization |

**Milestone:** RC1

---

## Sprint 11 — QA / UAT buffer

**Goal:** Release candidate

| Work |
|------|
| QA full cycle ([testing-roadmap.md](./testing-roadmap.md)) |
| UAT with business |
| P0/P1 fixes only |
| Release notes draft |

**Milestone:** GA readiness

---

## Velocity assumptions

| If team size | Adjust |
|--------------|--------|
| 1 dev | Add 50–70% calendar time; merge sprints 7+8 |
| 4 devs | Parallelize B5/B6 after B4; shorten sprints 7–9 |
| No dedicated QA | Extend Sprint 11; start manual testing Sprint 6 |

---

## Sprint ceremony

| Ceremony | Frequency |
|----------|-----------|
| Sprint planning | Start — scope from this doc + playbook gates |
| Daily standup | Daily |
| Phase gate review | End — [phase-execution-playbook.md](./phase-execution-playbook.md) sign-off |
| Retro | End |

---

Related: [backend-development-phases.md](./backend-development-phases.md) · [implementation-roadmap.md](./implementation-roadmap.md)
