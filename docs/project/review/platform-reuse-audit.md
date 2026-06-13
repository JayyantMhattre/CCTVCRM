# Platform Reuse Audit — CF-1

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** CF-1 — Post-implementation platform reuse verification  
**Baseline:** Enterprise Base Template V1 (Ashraak Platform V1 — frozen)  
**Rule:** No duplicate platform functionality in CCTV implementation

**Prior validation:** [platform-reuse-validation.md](./platform-reuse-validation.md) (D1-0 design-time)  
**This audit:** Implementation-time verification against codebase and D1 completion reports

---

## 1. Audit verdict

| Check | Design-time (D1-0) | Implementation (CF-1) |
|-------|:------------------:|:---------------------:|
| No duplicate Auth | ✅ PASS | ✅ **CONFIRMED** |
| No duplicate Files | ✅ PASS | ✅ **CONFIRMED** |
| No duplicate Notifications core | ✅ PASS | ✅ **CONFIRMED** |
| No duplicate Audit | ✅ PASS | ✅ **CONFIRMED** |
| No duplicate Theme logic | ✅ PASS | ✅ **CONFIRMED** |
| No duplicate Mobile infrastructure | ✅ PASS | ✅ **CONFIRMED** |
| CCTV = business modules only | ✅ PASS | ✅ **CONFIRMED** |

**Platform reuse audit: APPROVED**

---

## 2. Authentication — no duplication

| Expected | Implementation evidence | Duplicate? |
|----------|-------------------------|:----------:|
| Platform OpenIddict / JWT | All portals use platform Auth; Bearer on API calls | ❌ |
| Password reset OTP | `POST /auth/password-reset/*` in Auth module only | ❌ |
| Web forgot/reset pages | `modules/auth/pages/` — platform Auth SPA | ❌ |
| Mobile forgot/reset | `features/auth/` — same Auth APIs, no parallel store | ❌ |
| CCTV custom auth module | **None found** | ❌ |

**Wave 4 note:** Mobile password reset added UI only; zero new auth backend. SMS OTP delivery deferred — Auth email channel only (V1.1).

---

## 3. Files — no duplication

| Expected | Implementation evidence | Duplicate? |
|----------|-------------------------|:----------:|
| Central file storage | `POST /api/v1/files`; `FileRecord` in Files module | ❌ |
| Web upload component | `@/shared/file-upload` (`FileUpload`, `uploadFile`) | ❌ |
| Visit evidence (photos, selfie, signature, video) | Upload → link via visit endpoints; FileId only | ❌ |
| Ticket/site attachments | Same two-step pattern | ❌ |
| Invoice PDF storage | Generated PDF uploaded via Files; `fileId` on invoice | ❌ |
| CCTV blob storage module | **None found** | ❌ |

**Wave 4 note:** Visit video uses existing `POST .../visits/{id}/attachments` + Files upload — no new storage subsystem.

---

## 4. Notifications — no duplication

| Expected | Implementation evidence | Duplicate? |
|----------|-------------------------|:----------:|
| Email dispatch | Platform `INotificationService.SendEmailAsync` | ❌ |
| Template engine | Platform templates under `Templates/cctv/` | ❌ |
| CCTV event handlers | `Ashraak.Cctv.Integration.Infrastructure/Notifications/` | ❌ (handlers only) |
| SMS provider | `ISmsProvider` in Integration.Application (EXTEND per ADR-CCTV-0001) | ❌ (not duplicate — platform has no SMS) |
| Separate CCTV notification service | **None** — `ICctvNotificationDispatcher` wraps platform email/SMS | ❌ |
| Push infrastructure | Mobile FCM provider reused; Wave 4 added `DeepLink` payload keys only | ❌ |

**Wave 4 note:** Deep links added to notification `Data()` maps — no new notification transport.

---

## 5. Audit — no duplication

| Expected | Implementation evidence | Duplicate? |
|----------|-------------------------|:----------:|
| Compliance capture | Platform Audit observer on domain events | ❌ |
| Admin audit viewer | Platform audit screens (web + mobile) REUSE | ❌ |
| Business history tables | CCTV-owned (`TicketStatusHistory`, `VisitApproval`, etc.) — **not** duplicate audit store | ❌ |
| CCTV MongoDB audit module | **None found** | ❌ |

**Known limitation (accepted V1):** Platform audit read API stub — documented in D1-0 M-07; domain histories provide forensics.

---

## 6. Theme logic — no duplication

| Expected | Implementation evidence | Duplicate? |
|----------|-------------------------|:----------:|
| Theme Engine / platform-ui | CCTV pages use shared components (`PageHeader`, `Spinner`, cards) | ❌ |
| No direct CoreUI in CCTV | Grep: no `@coreui` imports under `modules/cctv/` | ❌ |
| No CCTV theme fork | No `cctv-theme` or adapter duplication | ❌ |
| Public website structure | Uses existing SPA shell + Bootstrap-style layout via platform patterns | ❌ |

---

## 7. Mobile infrastructure — no duplication

| Expected | Implementation evidence | Duplicate? |
|----------|-------------------------|:----------:|
| GoRouter + guards | `app_router.dart` extends platform shell | ❌ |
| Auth session / token storage | `core/auth/` — platform foundation | ❌ |
| HTTP client | `BaseApiClient` + interceptors (JWT, refresh, correlation) | ❌ |
| Offline queue | `core/offline/` + `CctvOfflineQueue` — extends platform pattern | ❌ |
| Deep links | `core/navigation/deep_links/` — Wave 4 extended parser only | ❌ |
| Push | `fcm_push_provider.dart` — platform provider | ❌ |
| Files upload/download | `features/files/` — platform feature reused | ❌ |
| Parallel mobile auth SDK | **None** — same `/api/v1` paths as web | ❌ |

---

## 8. Other platform surfaces — verified REUSE

| Surface | Usage | Duplicate? |
|---------|-------|:----------:|
| Webhooks | Admin/mobile webhook screens unchanged | ❌ |
| API keys | Platform module REUSE | ❌ |
| Sessions | Platform sessions API + mobile page | ❌ |
| Tenant settings | Platform module REUSE | ❌ |
| Users / profile | Platform Users module | ❌ |
| Caching | Platform cache for password-reset OTP | ❌ |
| CI/CD | Existing GitHub workflows extended | ❌ |

---

## 9. Correctly NEW (not duplicates)

These are **expected** CCTV-owned capabilities — platform has no equivalent:

| Capability | Location | Duplicate of platform? |
|------------|----------|:----------------------:|
| PDF generation (QuestPDF) | Integration.Infrastructure | ❌ — ADR-CCTV-0002 |
| SMS adapter | Integration.Infrastructure | ❌ — ADR-CCTV-0001 |
| 8 business domain modules | `Modules/Cctv/*` | ❌ |
| Reporting data provider | Integration + Reporting | ❌ |
| Public/portal CCTV pages | `FrontEnd/apps/web/modules/cctv/` | ❌ (uses platform shell) |
| Mobile CCTV feature slices | `FrontEnd.Mobile/features/cctv_*` | ❌ |

---

## 10. Wave 4 reuse compliance

| Wave 4 change | Reuse pattern | Compliant? |
|---------------|---------------|:----------:|
| Reporting filters/pagination | Extended existing Reporting API + UI | ✅ |
| Invoice admin buttons | Existing MediatR commands | ✅ |
| Mobile password reset | Platform Auth endpoints | ✅ |
| Visit video | Files + attachments endpoint | ✅ |
| Push deep links | Extended parser + notification data | ✅ |
| Public content | Static `content.ts` only | ✅ |

**No Wave 4 item introduced duplicate platform modules.**

---

## 11. Findings and flags

| ID | Finding | Severity | Action |
|----|---------|----------|--------|
| PR-01 | No duplicate platform modules detected | — | None |
| PR-02 | Auth SMS OTP not wired (email only) | Low | V1.1 deferred — not a reuse violation |
| PR-03 | FCM backend dispatch incomplete | Low | V1.1 — mobile handler reuses platform push infra |
| PR-04 | Traceability doc lag on reuse classifications | Low | Doc hygiene in testing phase |

---

## 12. Conclusion

Implementation **conforms to freeze §20 and D1-0 platform reuse validation**. CCTV correctly schedules business domain work on top of frozen platform capabilities. Wave 4 changes extended existing infrastructure without forking or duplicating Auth, Files, Notifications, Audit, Theme, or Mobile foundation.

**Platform reuse audit: APPROVED for code freeze.**

---

Related: [code-freeze-review.md](./code-freeze-review.md) · [architecture-signoff-report.md](./architecture-signoff-report.md)
