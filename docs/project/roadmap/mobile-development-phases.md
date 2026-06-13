# Mobile Development Phases

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Foundation (REUSE):** Flutter Mobile Platform V1 — auth, files, offline, sync, push, SDK, fastlane ([mobile-platform-manifest.md](../../mobile/mobile-platform-manifest.md))

Legend: **R** REUSE existing capability · **E** EXTEND · **N** NEW feature slice

---

## M0 — SDK & app packaging (D1 / start Sprint 9 prep)

| Deliverable | R/E/N |
|-------------|:-----:|
| Regenerate `api_client` with `/api/v1/cctv/*` | E |
| Customer vs Engineer app flavor/build config | E |
| Deep link routes for CCTV screens | E |
| `features/cctv_*` folder structure | N |

**Reuse:** `core/api`, `core/auth`, `core/offline`, `core/sync`, `core/notifications`

---

## M1 — Customer Mobile App (Sprint 9)

| # | Screen | R/E/N | Capability reused |
|---|--------|:-----:|-------------------|
| 1–2 | Login, password reset | **R** | `features/auth` |
| 3 | Dashboard | **N** | SDK + shell |
| 4–7 | AMC, plan, PDFs, renewal | **N** | SDK; **R** files preview |
| 8–9 | Upcoming visits, service history | **N** | SDK; **R** PDF |
| 10–12 | Tickets list/create/detail | **N** | SDK; **R** camera/gallery upload |
| 13–14 | Invoices + PDF | **N** | **R** files |
| 15 | Notifications list | **E** | push + **N** list UI |
| 16–18 | Prefs, profile, sessions | **R** | platform features |

**Offline:** Online-first (no write queue)  
**APIs:** [mobile-api-consumption.md](../design/mobile-api-consumption.md)

---

## M2 — Engineer Mobile App (Sprint 9)

| # | Screen | R/E/N | Capability reused |
|---|--------|:-----:|-------------------|
| 19 | Login + biometrics | **R** | `features/auth` |
| 20–22 | Home, visits list, detail | **N** | **R** offline cache read |
| 23–28 | Reporting + evidence + submit | **N** | **R** camera, GPS, **R** sync queue |
| 24–27 | Photos, selfie, GPS, signature | **N** | **R** files upload; **N** SignatureCanvas |
| 29 | Returned reports | **N** | SDK |
| 30–32 | Tickets | **N** | SDK |
| 33 | Sync status | **N** UI on **R** sync core | 
| 34 | Profile/sessions | **R** | platform |

**Offline:** Full visit capture queue; idempotent `POST /engineer/visits/sync`  
**Critical tests:** BR-VISIT-01 server re-validation; conflict handling

---

## M3 — Release engineering (end Sprint 9 / REL)

| Deliverable | R/E/N |
|-------------|:-----:|
| Beta via fastlane | **R** |
| Store assets update | **E** |
| Push notification production config | **E** |
| Crash reporting symbols | **R** |

**Reuse:** Existing `mobile.yml`, android/ios release workflows

---

## Classification summary

| Class | Customer app | Engineer app |
|-------|:------------:|:------------:|
| REUSE | 8 screens | 3 screens + infra |
| EXTEND | 1 | 2 (push, flavors) |
| NEW | 9 feature screens | 12 feature screens |

**No new mobile infrastructure scheduled** — no custom HTTP stack, no duplicate auth, no new offline engine.

---

## Mobile phase dependencies

| Phase | Requires |
|-------|----------|
| M1 Customer | B2–B6 APIs + `/portal/*` endpoints |
| M2 Engineer | B4 visit APIs + sync endpoint + B5 tickets |
| M3 Release | QA mobile tier pass |

---

## Mobile DoD

- [ ] All screens match [mobile-screen-design.md](../design/lld/mobile-screen-design.md)
- [ ] Generated SDK only — no hand DTOs
- [ ] Engineer offline sync E2E test
- [ ] Push deep links verified
- [ ] Beta build via existing fastlane

---

Related: [frontend-development-phases.md](./frontend-development-phases.md) · [testing-roadmap.md](./testing-roadmap.md)
