# Definition of Done

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Governance:** [documentation-governance.md](../../documentation-governance.md) · [documentation-pr-checklist.md](../../documentation-pr-checklist.md)

Work is **not done** until all applicable sections below pass.

---

## 1. Backend Definition of Done

| # | Criterion |
|---|-----------|
| B1 | Code merged to `main` via approved PR |
| B2 | Follows Microsoft C# conventions + project `.cursor/rules` |
| B3 | Module respects layer boundaries (Architecture tests pass) |
| B4 | **No changes to frozen platform module code** (Auth, Files, etc.) |
| B5 | All endpoints in scope documented in module `api.md` with OpenAPI metadata |
| B6 | Permissions enforced on every endpoint ([rbac-matrix.md](../design/rbac-matrix.md)) |
| B7 | FluentValidation for all commands; business rules have tests |
| B8 | Domain events published; Outbox wired |
| B9 | EF migration included (if schema change) — additive only V1 |
| B10 | Integration test(s) for primary happy path + key rule violations |
| B11 | Notification handlers for §17 events in scope (if applicable) |
| B12 | Webhook catalog updated for new events |
| B13 | No secrets committed; config via appsettings/env |
| B14 | CI green |

---

## 2. Frontend Definition of Done

| # | Criterion |
|---|-----------|
| F1 | Screens match [screen-design-specification.md](../design/lld/screen-design-specification.md) |
| F2 | Forms match [form-catalog.md](../design/lld/form-catalog.md) + [validation-rules.md](../design/lld/validation-rules.md) |
| F3 | Grids match [grid-catalog.md](../design/lld/grid-catalog.md) |
| F4 | **Imports only from `@/platform-ui` and `@/shared/*`** — no theme/vendor imports |
| F5 | Routes registered with correct AuthGuard/RoleGuard/PermissionGuard |
| F6 | Nav items in platform `navigationConfig.ts` |
| F7 | TanStack Query for server state; no ad-hoc fetch |
| F8 | File uploads use shared `FileUpload` component |
| F9 | Toasts per [notification-ux-design.md](../design/lld/notification-ux-design.md) |
| F10 | Responsive per [theme-usage-design.md](../design/lld/theme-usage-design.md) |
| F11 | Component or E2E test for primary workflow |
| F12 | CI green (lint + build + tests) |

---

## 3. Mobile Definition of Done

| # | Criterion |
|---|-----------|
| M1 | Screens match [mobile-screen-design.md](../design/lld/mobile-screen-design.md) |
| M2 | Uses generated OpenAPI SDK only — no hand-written API models |
| M3 | Reuses platform features (auth, files, offline, sync, push) |
| M4 | Engineer offline path tested on device |
| M5 | Deep links work from push notifications (if in scope) |
| M6 | No duplicate mobile infrastructure |
| M7 | `mobile.yml` CI green |
| M8 | Beta build produced via existing fastlane |

---

## 4. Documentation Definition of Done

| # | Criterion |
|---|-----------|
| D1 | **No feature without docs** (governance rule) |
| D2 | New/changed module has 7 files: README, architecture, registration, api, events, extending, operations |
| D3 | `docs/architecture/module-map.md` updated if module added |
| D4 | Significant decisions have ADR in `docs/adr/` or `docs/project/adr/` |
| D5 | OpenAPI changes reflected in endpoint-catalog or module api.md |
| D6 | `docs/index.md` updated if new doc sections |
| D7 | docs-validation CI pass |

---

## 5. Testing Definition of Done

| # | Criterion |
|---|-----------|
| T1 | Unit tests for new domain/validation logic |
| T2 | Integration test for module API (minimum) |
| T3 | Architecture tests pass |
| T4 | No decrease in coverage without approval |
| T5 | E2E updated if critical path changed ([testing-roadmap.md](./testing-roadmap.md)) |
| T6 | Manual test notes in PR if automation not feasible |

### Review Gate 1 — D1-1 through D1-5 (test execution deferred)

During D1-1 … D1-5, **create** tests (T1, T2, T5 as applicable) but **do not require execution** for phase sign-off.

| Required at phase exit | Deferred to Review Gate 2 |
|------------------------|---------------------------|
| T3 — Architecture tests pass | T1 — Unit test execution |
| Tests committed in repo | T2 — Integration test execution |
| | T4 — Coverage gates |
| | T5 — E2E execution |
| | T6 — Manual UAT scripts |

See [phase-execution-playbook.md](./phase-execution-playbook.md) — Review Gate 1 and Review Gate 2.

### Review Gate 2 — After D1-5 (full test execution)

All Testing DoD items (T1–T6) apply before starting D1-6+ (Tickets, Invoices, Portals, Mobile).

---

## 6. Release Definition of Done

| # | Criterion |
|---|-----------|
| R1 | Phase playbook gates signed ([phase-execution-playbook.md](./phase-execution-playbook.md)) |
| R2 | All P0/P1 defects closed |
| R3 | UAT sign-off for V1 |
| R4 | Release notes published |
| R5 | Migration runbook verified on staging |
| R6 | Rollback procedure tested |
| R7 | Production smoke pass post-deploy |
| R8 | Hypercare owner assigned (2 weeks) |

---

## 7. Phase completion checklist (summary)

Use at each B1–B7 / sprint gate.

**D1-1 through D1-5 (Review Gate 1):**

```
☐ dotnet restore success
☐ dotnet build success
☐ Architecture tests pass (T3)
☐ Documentation DoD (module docs + index + completion report)
☐ Unit/integration/E2E tests created where appropriate (execution deferred)
☐ Phase completion report published
☐ Playbook Review Gate 1 sign-off
```

**After D1-5 (Review Gate 2) — before D1-6+:**

```
☐ Full Testing DoD (T1–T6) executed and green
☐ CI test stage green
☐ Review Gate 2 sign-off
```

**All other phases (B5–B7, REL):**

```
☐ Backend DoD (applicable items)
☐ Frontend DoD (applicable items)
☐ Mobile DoD (if sprint includes mobile)
☐ Documentation DoD
☐ Testing DoD (full execution)
☐ Demo recorded / notes
☐ Playbook sign-off
```

---

## 8. Explicit exclusions (not required for Done)

| Item | Reason |
|------|--------|
| Re-test entire platform Auth module | Frozen + already tested |
| Rebuild audit storage | Platform observer |
| Theme migration of legacy platform pages | Out of CCTV scope |
| Payment gateway | Out of scope §21 |
| Chart library for reports | V1 uses tables ([dashboard-specification.md](../design/lld/dashboard-specification.md)) |

---

Related: [phase-execution-playbook.md](./phase-execution-playbook.md) · [testing-roadmap.md](./testing-roadmap.md)
