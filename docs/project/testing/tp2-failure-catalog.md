# TP-2 Failure Catalog

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** TP-2 — Automated test execution  
**Policy:** Failures documented only — **no fixes applied**

---

## Summary

| Category | Test / Step failures | Environment / harness failures | Product defects |
|----------|---------------------:|---------------------------------:|----------------:|
| Backend | **0** | 3 (coverage, empty projects, TRX) | **0** |
| Architecture | **0** | 0 | **0** |
| Frontend | **0** (tests not run) | **3** | **0** |
| Mobile | **0** (not run) | **1** | **0** |
| Database | **0** (not run) | **5** | **0** |
| **Total blocking product failures** | | | **0** |

All recorded failures are **environment or test-harness gaps**, not confirmed application defects.

---

## 1. Backend

### 1.1 Test failures

**None.** 152/152 tests passed.

### 1.2 Harness / infrastructure issues

| ID | Type | Description | Severity | TP-4 fix? |
|----|------|-------------|----------|-----------|
| BE-H-01 | Coverage | `XPlat Code Coverage` datacollector not installed — no coverage output | Medium | Harness only |
| BE-H-02 | Empty projects | `Ashraak.Auth.Tests`, `Ashraak.Users.Tests`, `Ashraak.Audit.Tests` report "No test is available" | Low | Optional test authoring (post-V1) |
| BE-H-03 | TRX | Single `backend-tests.trx` overwritten per test assembly — last file = ApiKeys (7 tests) only | Low | CI harness improvement |

### 1.3 Warnings

| ID | Description |
|----|-------------|
| BE-W-01 | Build: 0 warnings |
| BE-W-02 | Test run: multiple "Overwriting results file" warnings |

---

## 2. Architecture

### 2.1 Test failures

**None.** 21/21 architecture validation tests passed.

### 2.2 Verified rules (all pass)

- SharedKernel and SharedKernel.Contracts module isolation
- Platform domain layer purity (Auth, Tenant, Users, Audit)
- Auth ↔ Users / Tenant ↔ Auth cross-module boundaries
- CommandHandler and Repository internal visibility
- CCTV domain layer purity (8 modules)
- CCTV lead domain must not reference other CCTV domains
- CCTV Integration.Application must not reference CCTV Infrastructure
- CCTV notification template key registry completeness

### 2.3 Issues

**None.**

---

## 3. Frontend

### 3.1 Test failures

Vitest did not execute — **0 tests run**, **0 test failures**.

### 3.2 Step failures

| ID | Step | Error | Root cause | Classification |
|----|------|-------|------------|----------------|
| FE-F-01 | Lint | `'eslint' is not recognized as an internal or external command` | `eslint` not in `package.json` devDependencies; binary missing from `node_modules/.bin` | **Harness gap** |
| FE-F-02 | Vitest | `SyntaxError: Unexpected token '??='` in vitest CAC module | Node **v14.17.3** — Vitest 3 requires Node 18+ | **Environment gap** |
| FE-F-03 | Production build | `SyntaxError: Unexpected token '||='` in Vite 6 | Node **v14.17.3** — Vite 6 requires Node 18+ | **Environment gap** |

### 3.3 Passed steps

| ID | Step | Result |
|----|------|--------|
| FE-P-01 | Type check (`tsc --noEmit`) | ✅ Pass — 0 TypeScript errors |

### 3.4 Warnings

| ID | Description |
|----|-------------|
| FE-W-01 | Node.js v14.17.3 on agent — below documented requirement (Node 20+ for CI parity) |
| FE-W-02 | `npm run build` process exit code 0 despite Vite failure (unhandled promise rejection) — misleading exit code |

---

## 4. Mobile

### 4.1 Test failures

**Not executed** — 0 tests run, 0 failures.

### 4.2 Environment failures

| ID | Step | Error | Root cause | Classification |
|----|------|-------|------------|----------------|
| MO-F-01 | All mobile steps | `flutter : The term 'flutter' is not recognized` | Flutter SDK not installed / not on PATH | **Environment gap** |

### 4.3 Expected scope (deferred)

| Area | Files | Notes |
|------|------:|-------|
| Unit/widget tests | 23 | Includes CCTV deep-link parser tests |
| `flutter analyze --fatal-infos` | — | Defined in `.github/workflows/mobile.yml` |
| Extended CI subset | — | offline, sync, navigation, feature_flags, biometrics, notifications |

---

## 5. Database

### 5.1 Migration / startup failures

**Not executed** — no runtime failures observed.

### 5.2 Environment failures

| ID | Step | Error / blocker | Classification |
|----|------|-----------------|----------------|
| DB-F-01 | Restore staging copy | No staging backup path or credentials on agent | **Environment gap** |
| DB-F-02 | Apply migrations | `docker` command not available — cannot start local PostgreSQL stack | **Environment gap** |
| DB-F-03 | Verify API startup | Depends on DB-F-02 | **Blocked by DB-F-02** |
| DB-F-04 | Verify seed data (`CctvRbacSeedHostedService`) | Requires running API + DB | **Blocked by DB-F-02** |
| DB-F-05 | Verify module registration | Requires running API | **Blocked by DB-F-02** |

### 5.3 Static verification only

| Item | Status |
|------|--------|
| CCTV migration files exist (Lead → Invoice) | ✅ Confirmed in repo |
| Platform migration projects present | ✅ Confirmed in repo |
| Runtime migrate + rollback | ❌ Not executed |

**Maps to freeze condition C-04:** ❌ Not satisfied.

---

## 6. Cross-cutting gaps (not failures)

| ID | Gap | Impact |
|----|-----|--------|
| X-01 | No GitHub CLI (`gh`) on agent — cannot pull CI artifact history | Reporting only |
| X-02 | Frontend ESLint missing from dependencies while lint script exists | Lint step always fails until harness fixed |
| X-03 | Node 14 on Windows dev agent vs Vite 6 / Vitest 3 requirements | Blocks web vitest + build locally |

---

## 7. Defect triage guidance (TP-3 / TP-4)

Per [defect-management-process.md](./defect-management-process.md):

- **Do not** open product defects for FE-F-02, FE-F-03, MO-F-01, DB-F-* until re-run on correct environment.
- **Do** open harness tasks (not product defects) for ESLint dependency and Node version pinning if not already tracked.
- Backend test pass provides high confidence for domain logic; manual smoke (TP-3) still required for UI and DB-integrated flows.

---

*Failure catalog complete. No code changes made in TP-2.*
