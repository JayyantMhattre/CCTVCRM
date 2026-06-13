# D1-13f — Admin & Auth UX Completion Report

**Date:** 2026-06-11  
**Wave:** D1-13 Wave 3  
**Scope:** Admin UX completion + customer password reset OTP (V1 freeze only)

---

## Objective

Complete remaining administrative and authentication UX gaps using existing platform APIs and Auth architecture — no parallel auth flows, no new domain features.

---

## Delivered

### Engineer admin UX (`FrontEnd/apps/web`)

| Capability | Page / route |
|------------|--------------|
| Create engineer | `EngineerCreatePage` → `/admin/engineers/new` |
| Edit engineer + deactivate + workload | `EngineerDetailPage` (inline edit, workload panel, status toggle) |
| User mapping | Platform user picker on create/edit |
| List entry | “New engineer” on `EngineerListPage` |

Routes registered in `modules/cctv/routes.tsx`.

### Invoice admin UX

| Capability | Page / route |
|------------|--------------|
| Create draft invoice | `InvoiceDraftPage` → `/admin/invoices/new` |
| Edit draft invoice | `InvoiceDraftPage` → `/admin/invoices/:invoiceId/edit` |
| Preview draft | Link to `InvoiceDetailPage` |
| Generate invoice | Generate action on draft page + detail page |

Extended `invoices/api.ts`: `update`, `generate`, `send`, `markPaid`, `cancel`.

### Site admin UX

| Capability | Implementation |
|------------|----------------|
| Contact editor | Inline upsert form on `SiteDetailPage` (max 3 contacts) |
| Document upload | `FileUpload` → Files module → POST `/cctv/sites/{id}/documents` |
| Document management | List + remove on `SiteDetailPage` |

Extended `sites/api.ts`: `getDocuments`, `linkDocument`, `removeDocument`.

### Customer password reset (platform Auth)

**Backend** (`Ashraak.Auth.Application` + `AuthExtendedEndpoints`):

- `POST /api/v1/auth/password-reset/request` — email OTP (6-digit, 10 min cache), audit via `RequestPasswordReset`
- `POST /api/v1/auth/password-reset/verify` — returns `challengeId`
- `POST /api/v1/auth/password-reset/confirm` — change password, revoke sessions
- Email template: `Templates/password-reset-otp.txt`

**Frontend** (`FrontEnd/apps/web`):

- `ForgotPasswordPage` → `/forgot-password`
- `ResetPasswordPage` → `/reset-password`
- `passwordResetApi.ts`
- Login page “Forgot password?” link
- Portal profile card linking to forgot-password flow

Mobile OTP: request API accepts optional `phoneNumber`; SMS channel uses platform `INotificationService` when configured (same as other OTP flows).

---

## Requirements closed

| ID | Requirement | Status |
|----|-------------|--------|
| FR-ENG-01 | Engineer master CRUD + user mapping + workload | **Implemented** (admin web) |
| FR-INV-01 | Invoice draft create/edit/generate | **Implemented** (admin web) |
| FR-SITE-02/03 | Site contact edit + document linking | **Implemented** (admin web) |
| FR-CUST-03 / FR-CP-01h | Customer password reset OTP | **Implemented** (Auth API + web UI) |

---

## Remaining gaps

| Gap | Notes |
|-----|-------|
| Invoice send / mark paid / cancel UI | APIs wired in client; admin detail actions can be added in polish pass |
| Mobile native forgot-password screens | Web SPA flow complete; Flutter login shell unchanged |
| SMS OTP for password reset | Backend accepts phone; depends on tenant SMS provider configuration |
| Reporting LLD polish | D1-13g (post Wave 3) |

---

## Verification

| Check | Result |
|-------|--------|
| Backend build (`Ashraak.Api`) | ✅ Succeeded (after Auth handler fix) |
| Architecture tests | ✅ 21/21 passed |
| Frontend TypeScript build | ⚠️ Not run in agent (no `npm` step in this session) |

---

## Audit

All administrative mutations continue through existing audited command handlers (engineer, invoice, site, auth domain events).

---

*Wave 3 stream D1-13f complete. STOP — Wave 4 not started.*
