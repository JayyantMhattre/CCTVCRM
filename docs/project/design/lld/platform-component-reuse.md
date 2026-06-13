# Platform Component Reuse

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7 — **most important LLD document**
**Rule:** Business modules import **`@/platform-ui` and `@/shared/*` only** — never `@coreui/*`, theme adapters, or vendor UI ([theme decision record](../../../frontend/themes/theme-decision-record.md))

Classification: **REUSE** (use as-is) · **EXTEND** (wrap/configure for CCTV) · **NEW** (CCTV-only composite on platform primitives)

---

## 1. Web platform inventory

| Platform asset | Location | CCTV usage | Class |
|----------------|----------|------------|:-----:|
| `PlatformLayout` / `PlatformAuthLayout` | `platform-ui/layout` | All portal shells | REUSE |
| `usePlatformNav` / `PlatformNavRenderer` | `platform-ui/navigation` | CCTV nav items registered in platform config | EXTEND |
| `AuthGuard` / `RoleGuard` / `PermissionGuard` | `shared/guards` | All protected routes | REUSE |
| `PlatformCard` | `platform-ui/cards` | Every page panel, dashboard widgets | REUSE |
| `PlatformTable<TRow>` | `platform-ui/tables` | All admin/customer/engineer lists | REUSE |
| `PlatformPagination` | `platform-ui` | Server-side paginated grids | REUSE |
| `PlatformFormField` | `platform-ui/forms` | All forms (wraps RHF field) | REUSE |
| `PlatformDialog` / `PlatformConfirmDialog` | `platform-ui/dialogs` | Confirm delete, approve/return visit | REUSE |
| `PlatformBadge` | `platform-ui/badges` | Status, priority, invoice type pills | REUSE |
| `PlatformTabs` / `PlatformTab` | `platform-ui/tabs` | Site detail, contract detail, ticket detail | REUSE |
| `PlatformBreadcrumb` | `platform-ui/breadcrumbs` | Admin deep pages | REUSE |
| `PlatformAvatar` | `platform-ui/avatar` | Engineer/customer display | REUSE |
| `useToast` / `toastService` | `shared/ui/toast` | Success/error feedback | REUSE |
| `useApiError` / error classifier | `shared/errors` | API ProblemDetails → toast | REUSE |
| `FileUpload` | `shared/file-upload` | All attachment flows | REUSE |
| Audit viewer page | `modules/audit` | Admin Administration group | REUSE |
| Login / Register pages | `modules/auth` | Public + portal entry | REUSE |
| User list / profile | `modules/users` | Admin user mgmt + profile | REUSE |
| Tenant settings | `modules/tenant` | Administration | REUSE |
| Webhooks / ApiKeys modules | `modules/webhooks`, `apikeys` | Administration | REUSE |
| Notification preferences page | `modules/notifications` | Profile area | REUSE |
| Sessions page | `modules/auth/sessions` | Profile/security | REUSE |
| TanStack Query hooks pattern | module `api.ts` + hooks | All CCTV data fetching | REUSE (pattern) |
| react-hook-form + Zod | module form schemas | All CCTV forms | REUSE (pattern) |
| `PlatformChart` | `platform-ui/charts` | Reports hub (optional V1) | REUSE (deferred data) |

---

## 2. NEW CCTV composites (built on platform primitives only)

| Component | Purpose | Built from |
|-----------|---------|------------|
| `CctvStatusPipeline` | Lead pipeline kanban/counts | `PlatformCard` + `PlatformBadge` |
| `CctvEvidenceChecklist` | Visit completion progress | `PlatformCard` + checklist rows + `PlatformBadge` |
| `CctvSignaturePad` | Customer signature capture (web engineer portal) | Canvas wrapper + `FileUpload` |
| `CctvGpsCaptureButton` | Browser geolocation for web visits | Button + readonly fields |
| `CctvInvoiceLineEditor` | Editable invoice lines grid | `PlatformTable` inline edit + RHF field array |
| `CctvTermHistoryTable` | Admin contract term history | `PlatformTable` |
| `CctvTicketTimeline` | Status history + comments | Vertical list in `PlatformCard` |
| `CctvQuickActionsBar` | Admin dashboard shortcuts | Button group in `PlatformCard` |
| `CctvOfflineSyncBanner` | Engineer mobile-web sync state | `PlatformBadge` + alert pattern |

**Do not** create parallel design systems (no MUI, Ant Design, raw Bootstrap in CCTV modules).

---

## 3. Screen → component mapping (summary)

| Screen type | REUSE | EXTEND | NEW composite |
|-------------|-------|--------|---------------|
| List/grid screens | `PlatformTable`, `PlatformPagination`, filters in `PlatformCard` | Column defs per module | Export button (optional) |
| Create/edit forms | `PlatformFormField`, RHF, Zod, `FileUpload` | Schemas per entity | — |
| Detail pages | `PlatformCard`, `PlatformTabs`, `PlatformBadge` | Tab content per module | `CctvTicketTimeline`, `CctvTermHistoryTable` |
| Dashboards | `PlatformCard`, platform audit tile | Admin dashboard page | All CCTV KPI widgets |
| Visit reporting | `FileUpload`, `PlatformFormField` | Photo category tabs | `CctvEvidenceChecklist`, `CctvSignaturePad` |
| Workflow wizards | `PlatformDialog`, stepped `PlatformCard` | Convert lead wizard | — |
| Public website forms | `PlatformFormField` or static HTML forms | Inquiry forms | — |
| Platform admin screens | Entire existing modules | — | — |

Full per-screen mapping: [screen-design-specification.md](./screen-design-specification.md).

---

## 4. Mobile platform inventory (Flutter)

| Platform feature | Path | CCTV usage | Class |
|------------------|------|------------|:-----:|
| Auth + biometrics | `features/auth` | Login OTP | REUSE |
| Secure storage | `core/auth` | Tokens | REUSE |
| OpenAPI SDK | `packages/api_client` | All CCTV API calls | REUSE |
| Files upload/preview | `features/files` | Photos, PDFs, attachments | REUSE |
| Profile / sessions | `features/profile`, `features/sessions` | Profile tab | REUSE |
| Notification prefs | `features/notifications` | Email toggle | REUSE |
| Push | `core/notifications` | Event alerts | EXTEND (CCTV routes) |
| Offline cache | `core/offline` | Engineer visit reads | REUSE |
| Background sync | `core/sync` | Engineer visit submit queue | REUSE |
| Deep links | `core/navigation/deep_links` | Push → ticket/visit | EXTEND |
| Camera / gallery | Files feature UX | Visit evidence | REUSE |
| Correlation banner | `core/api` | Error display | REUSE |

NEW mobile widgets (Flutter, on platform theme): `VisitChecklistTile`, `SignatureCanvas`, `GpsCaptureTile`, `SyncQueueList` — styled with app theme tokens, not vendor-specific business UI.

---

## 5. Anti-patterns (forbidden)

| Forbidden | Use instead |
|-----------|-------------|
| Import `@coreui/react` in CCTV modules | `@/platform-ui` |
| Custom file upload POST | `FileUpload` → `/api/v1/files` |
| Custom toast library | `useToast` |
| Custom auth/login | Platform auth pages |
| Custom audit log storage UI | Platform audit viewer + business timelines |
| Hand-written API types in mobile | Generated SDK |
| Inline `<table>` for data grids | `PlatformTable` |
| Theme-specific business layouts | `PlatformLayout` only |

---

## 6. Implementation checklist (per CCTV screen)

1. Register route in central router + `PermissionGuard` / `RoleGuard`
2. Add nav item to `navigationConfig.ts` (platform-owned)
3. Page uses `PlatformCard` wrapper + `PlatformBreadcrumb` where deep-linked
4. Lists use `PlatformTable` + server pagination query params
5. Forms use Zod schema + `PlatformFormField` + mutation toast
6. Files use two-step `FileUpload` then link API
7. No theme imports — verify with architecture test / lint rule

---

Related: [theme-usage-design.md](./theme-usage-design.md) · [screen-design-specification.md](./screen-design-specification.md) · [form-catalog.md](./form-catalog.md) · [grid-catalog.md](./grid-catalog.md)
