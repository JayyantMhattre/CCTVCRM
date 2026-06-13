# D1-13i — Final Scope Completion Report (Push, Invoice, Auth, Public)

**Date:** 2026-06-12  
**Wave:** D1-13 Wave 4  
**Scope:** Push deep links, invoice admin actions, native mobile forgot password, public website content import

---

## Objective

Close remaining V1 polish items from the completeness review without new requirements or architecture redesign.

---

## D1-13i — Push notification deep links

### Backend

Added `DeepLink` payload key to CCTV notification data via `CctvNotificationHandlerSupport`:

| Event | Deep link target |
|-------|------------------|
| Ticket created / assigned / closed | `ashraak://cctv/customer/tickets/{id}` (customer); engineer assigned → tickets list |
| Visit scheduled | `ashraak://cctv/customer/visits` |
| Visit completed / approved | `ashraak://cctv/customer/service-history` |
| Engineer assigned | `ashraak://cctv/engineer/visits/{id}/report` (when visit exists) |
| Invoice generated | `ashraak://cctv/customer/invoices/{id}` |
| AMC renewal requested | `ashraak://cctv/customer/amc` |
| AMC expiry reminder (job) | `ashraak://cctv/customer/amc` |

Handlers updated: ticket, service, invoice, AMC notification handlers + `CctvAmcExpiryReminderHostedService`.

### Mobile

| Item | Detail |
|------|--------|
| Parser | `DeepLinkParser` — `cctv/customer/*` and `cctv/engineer/*` routes |
| Types | Extended `DeepLinkType` enum for CCTV routes |
| Handler | Existing `DeepLinkHandler.handlePayload` reads `deepLink` from FCM tap payload |
| Tests | `deep_link_parser_test.dart` — ticket, invoice, visit, AMC, password-reset |

---

## Invoice admin actions

| Action | UI | API |
|--------|-----|-----|
| Send invoice | `InvoiceDetailPage` when status = Generated | `invoicesApi.send` |
| Mark paid | `InvoiceDetailPage` when status = Sent | `invoicesApi.markPaid` |
| Cancel | `InvoiceDetailPage` when Generated or Sent | `invoicesApi.cancel` |

---

## Native mobile forgot password

| Screen | Route | API |
|--------|-------|-----|
| Request OTP | `/forgot-password` | `POST /auth/password-reset/request` |
| Verify OTP + reset | `/reset-password` | `POST /auth/password-reset/verify`, `/confirm` |
| Entry point | `UnauthorizedPage` — Forgot password link | Reuses platform Auth (no parallel system) |

Files: `features/auth/pages/forgot_password_page.dart`, `reset_password_page.dart`, `password_reset_repository.dart`.

---

## Public website content import

Aligned `content.ts` and `HomePage.tsx` with www.aarvii.in messaging:

- Hero: “Secure Your World with AARVII”
- Stats: 500+ installations, 24/7 support, 5+ years, 100% satisfaction
- Services: CCTV Installation, AMC, 24/7 Support, System Upgrades
- Why choose: expert installation, quick response, advanced technology

Site structure unchanged — content-only update on Home, About, Services, AMC, Contact via shared `content.ts`.

---

## Verification

| Check | Result |
|-------|--------|
| Backend build | ✅ |
| Architecture tests | ✅ 21/21 |
| Web TypeScript (`tsc -b`) | ✅ |
| Flutter analyze | ⚠️ Not run (Flutter SDK not on build agent PATH) |

---

## References

- [d1-13-v1-scope-completion-report.md](./d1-13-v1-scope-completion-report.md)
- [project-completeness-review.md](./review/project-completeness-review.md)
