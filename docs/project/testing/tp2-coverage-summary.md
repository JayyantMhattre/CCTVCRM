# TP-2 Coverage Summary

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** TP-2 — Automated test execution

---

## 1. Coverage collection status

| Layer | Tool attempted | Result | Metrics available |
|-------|----------------|--------|:-----------------:|
| Backend (.NET) | `--collect:"XPlat Code Coverage"` | ❌ Collector not installed | **No** |
| Frontend (web) | Vitest (built-in coverage not invoked) | ❌ Tests did not run (Node 14) | **No** |
| Mobile (Flutter) | `flutter test --coverage` (not invoked) | ❌ Flutter SDK unavailable | **No** |

**Conclusion:** TP-2 did **not** produce quantitative line/branch coverage percentages. Assessment below is **qualitative**, based on test inventory and execution results from [tp2-automated-test-results.md](./tp2-automated-test-results.md).

---

## 2. Backend — qualitative coverage

### 2.1 Executed tests: 152

| Module / area | Tests | Executed | Est. domain coverage |
|---------------|------:|:--------:|---------------------|
| CCTV Integration | 66 | ✅ | **High** — all 9 CCTV test files pass |
| Architecture | 21 | ✅ | **Structural** — all layers enforced |
| Webhooks (platform) | 45 | ✅ | **High** — platform module |
| ApiKeys (platform) | 7 | ✅ | **Medium–High** |
| SharedKernel | 7 | ✅ | **Medium** |
| Tenant | 6 | ✅ | **Medium** |
| Auth | 0 | ⚠️ Empty project | **None (automated)** |
| Users | 0 | ⚠️ Empty project | **None (automated)** |
| Audit | 0 | ⚠️ Empty project | **None (automated)** |

### 2.2 CCTV domain test mapping

| Domain | Test file | Scenarios covered (high level) |
|--------|-----------|--------------------------------|
| Lead | `LeadDomainTests` | Pipeline states, validation |
| Lead → Customer | `LeadConversionIntegrationTests` | Orchestrated conversion |
| Customer | `CustomerDomainTests` | CRUD, invariants |
| Site | `SiteDomainTests` | Contacts, AMC-per-site rule |
| AMC | `AmcDomainTests` | Plans, contracts, terms |
| Service | `ServiceDomainTests` | Schedules, visits, evidence |
| Ticket | `TicketDomainTests` | Lifecycle, attachments |
| Engineer | `EngineerDomainTests` | CRUD, assignments |
| Invoice | `InvoiceDomainTests` | Option B lifecycle |
| Health | `CctvHealthContractTests` | API contract smoke |

### 2.3 Backend coverage gaps

| Gap | Risk | Mitigation phase |
|-----|------|------------------|
| No line/branch metrics (coverlet missing) | Unknown untested code paths | Install collector; re-run TP-2 on CI |
| Auth/Users/Audit empty test projects | Platform regressions undetected | Platform smoke in CI; optional post-V1 tests |
| No Testcontainers DB integration | SQL/migration runtime not validated | TP-3 staging + C-04 |
| Reporting module (Wave 4) | No dedicated integration tests noted | TP-3 manual reports smoke |
| Visit video link API | May lack dedicated test | TP-3 manual + API smoke |
| Notification handler E2E | Mocked in domain tests | TP-3 staging |
| API WebApplicationFactory RBAC matrix | Not present | TP-3 manual RBAC negative tests |

---

## 3. Frontend — qualitative coverage

### 3.1 Test inventory: 6 files (not executed)

| Area | Files | CCTV coverage |
|------|------:|:-------------:|
| Webhooks | 4 | Platform only |
| ApiKeys | 1 | Platform only |
| Theme | 2 | Platform only |
| **CCTV modules** | **0** | **None** |

### 3.2 Validation executed

| Check | Result | Coverage signal |
|-------|--------|-----------------|
| TypeScript compile (`tsc --noEmit`) | ✅ Pass | All TS modules type-check |
| Vitest | ❌ Not run | 0% runtime component coverage |
| Production build | ❌ Vite blocked | Bundle not verified on agent |

### 3.3 Frontend coverage gaps

| Gap | Risk | Mitigation |
|-----|------|------------|
| Zero CCTV Vitest tests | UI regressions (reports, invoices, visits) | TP-3 manual smoke |
| No Playwright E2E | Critical path UI untested automatically | TP-3 manual; future E2E |
| Lint not runnable | Style/security rules not enforced locally | Add eslint to devDependencies; CI |
| Node 14 agent | False negative for build/test | Node 20+ CI re-run |

---

## 4. Mobile — qualitative coverage

### 4.1 Test inventory: 23 files (not executed)

| Area | Files | CCTV-related |
|------|------:|:------------:|
| Deep link parser | 1 | ✅ CCTV routes |
| Platform (auth, offline, webhooks, etc.) | 22 | Partial |
| CCTV feature widgets/screens | 0 | **None** |

### 4.2 Mobile coverage gaps

| Gap | Risk | Mitigation |
|-----|------|------------|
| Flutter not run on agent | Unknown analyze/test failures | `mobile.yml` CI or local SDK |
| No CCTV widget tests | Engineer/customer UI untested | TP-3 device smoke |
| FCM push (partial V1.1) | Push E2E not automatable | Deferred register; parser test only |

---

## 5. Architecture coverage

Architecture tests provide **100% rule coverage** for defined invariants (21 rules):

- 7 platform architecture tests (+ 4 theory cases for domain layers)
- 10 CCTV architecture tests (8 domain modules + 2 integration rules)
- 1 notification template registry test
- 3 theory iterations counted in xUnit total = **21 tests**

This is **structural coverage**, not line coverage.

---

## 6. Database coverage

| Aspect | Automated coverage | TP-2 status |
|--------|-------------------|-------------|
| Migration scripts exist | Static (repo) | ✅ Verified |
| Migration apply at runtime | None local | ❌ Not executed |
| Seed data (RBAC) | Runtime only | ❌ Not executed |
| Cross-schema integration | Integration tests (mocked) | Partial |

---

## 7. Recommended coverage actions (post-TP-2, pre-TP-3)

These are **harness/environment** actions — not product code changes:

| Priority | Action | Owner |
|----------|--------|-------|
| P1 | Re-run web lint/vitest/build on **Node 20+** | DevOps / Frontend |
| P1 | Run `flutter analyze` + `flutter test` via CI or SDK install | Mobile / DevOps |
| P1 | Staging DB restore + migrate (C-04) | DevOps |
| P2 | Add `coverlet.collector` to test projects; archive Cobertura | DevOps |
| P2 | Pin Node version in frontend CI workflow (if missing) | DevOps |
| P3 | Add ESLint to web `devDependencies` | Frontend (harness) |

---

## 8. Coverage vs V1 scope

| V1 capability | Automated coverage level |
|---------------|-------------------------|
| Lead → Invoice domain logic | **High** (66 integration tests) |
| Module boundaries | **High** (21 architecture tests) |
| Platform webhooks/apikeys | **Medium–High** |
| Admin web UI (CCTV) | **Low** (type-check only) |
| Customer/Engineer portals | **Low** |
| Mobile apps | **Unknown** (not executed) |
| DB migrations at runtime | **Unknown** (not executed) |
| Manual smoke paths | **Planned TP-3** |

---

*Quantitative coverage unavailable in TP-2. Re-run with coverlet + Node 20+ + Flutter SDK for metrics.*
