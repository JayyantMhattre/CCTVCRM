# Frontend Development Phases

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Stack:** React 19 · TanStack Query · react-hook-form + Zod · `@/platform-ui` only
**Shell:** Platform Theme Engine · centralized router · guards ([routing-and-guards.md](../../frontend/routing-and-guards.md))

Legend: **R** REUSE · **E** EXTEND · **N** NEW

---

## FP-0 — Portal shell (D1, parallel)

| Deliverable | R/E/N |
|-------------|:-----:|
| Route trees `/admin`, `/portal`, `/engineer` | E |
| Role redirect after login | E |
| CCTV nav entries in `navigationConfig.ts` | E |
| OpenAPI TypeScript client setup | E |
| Module folder `modules/cctv/` structure | N |

**Reuse:** AuthGuard, RoleGuard, PermissionGuard, PlatformLayout, existing auth pages

---

## FP-1 — Admin: Lead (with B1)

| Screen # | Screen | R/E/N | Backend |
|----------|--------|:-----:|---------|
| 12 | Lead List | N | B1 |
| 13 | Lead Detail | N | B1 |
| 14 | Lead Conversion wizard | N | B1 |
| 8–9 | Public inquiry forms (website) | N | B1 |

**Components:** PlatformTable, PlatformFormField, PlatformDialog, FileUpload  
**LLD:** [form-catalog.md](../design/lld/form-catalog.md), [grid-catalog.md](../design/lld/grid-catalog.md)

---

## FP-2 — Admin: Customer · Site · Asset (with B2)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 15–17 | Customer list/detail/form | N |
| 18–20 | Site detail/form/asset summary | N |

**Components:** PlatformTabs (site detail), repeatable contact form (max 3)

---

## FP-3 — Admin: AMC (with B3)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 21–22 | Plan list/detail/versions | N |
| 23–25 | Contract list/detail/renew term | N |
| — | Renewal requests queue | N |

**Components:** CctvTermHistoryTable (N on PlatformTable)

---

## FP-4 — Admin: Scheduling · Visits (with B4)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 26–27 | Schedule list/detail/assign | N |
| 28–29 | Approval queue/review | N |

**Components:** Evidence gallery (Files download), approve/return dialogs

---

## FP-5 — Admin: Tickets · Engineers (with B5)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 30–32 | Ticket list/detail/create | N |
| 33–34 | Engineer list/detail | N |

**Components:** CctvTicketTimeline (N)

---

## FP-6 — Admin: Invoices (with B6)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 35–37 | Invoice list/draft/detail | N |

**Components:** CctvInvoiceLineEditor (N)

---

## FP-7 — Customer Portal (Sprint 7)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 46 | Dashboard | N |
| 47–48 | AMC + renewal | N |
| 49–50 | Visits/history | N |
| 51–53 | Tickets | N |
| 54–55 | Invoices | N |
| 56–57 | Profile/password | R |
| 58 | Notifications feed | E |

**Reuse:** Platform profile, auth flows, FileUpload for ticket attachments

---

## FP-8 — Engineer Portal (Sprint 8)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 59 | Home/My Day | N |
| 60–62 | Visits + reporting | N |
| 63–66 | Evidence capture | N (CctvEvidenceChecklist, CctvSignaturePad, CctvGpsCaptureButton) |
| 67–69 | Tickets | N |
| 70 | Profile | R |

**Mobile-first visit reporting** — web mirrors when online

---

## FP-9 — Admin: Reports · Dashboard polish (Sprint 10)

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 11 | Admin dashboard widgets | E |
| 38 | Reports hub + views | N |

**Reuse:** Platform audit tile, webhook/api-key cards ([dashboard-specification.md](../design/lld/dashboard-specification.md))

---

## FP-Admin — Administration group

| Screen # | Screen | R/E/N |
|----------|--------|:-----:|
| 39–45 | Users, Tenant, Audit, ApiKeys, Webhooks, Sessions, Profile | **R** |

**Zero CCTV implementation** — mount existing platform modules under `/admin` Administration nav.

---

## Classification summary

| Class | Admin business | Customer | Engineer | Platform admin |
|-------|:--------------:|:--------:|:--------:|:--------------:|
| REUSE | — | 2 | 1 | 7 |
| EXTEND | 1 (dashboard) | 1 | — | — |
| NEW | 31 | 10 | 11 | — |

---

## Frontend DoD per phase

- [ ] Routes in central router with guards
- [ ] Nav items permission-gated
- [ ] Zod schemas match [validation-rules.md](../design/lld/validation-rules.md)
- [ ] No theme/vendor imports ([theme-usage-design.md](../design/lld/theme-usage-design.md))
- [ ] API via generated client or typed `api.ts`
- [ ] Toast success/error per [notification-ux-design.md](../design/lld/notification-ux-design.md)
- [ ] E2E smoke for primary workflow of phase

---

Related: [mobile-development-phases.md](./mobile-development-phases.md) · [sprint-plan.md](./sprint-plan.md)
