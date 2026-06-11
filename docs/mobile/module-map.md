# Mobile module map

Maps **backend modules** → **web modules** → **Flutter features**.

Update alongside [mobile-platform-manifest.md](./mobile-platform-manifest.md).

---

## Feature ↔ API mapping

| Flutter feature | Backend module | Primary APIs | Web reference | Mobile status |
|-----------------|----------------|--------------|---------------|---------------|
| `features/auth` | Auth | `/connect/token`, register, MFA | `modules/auth` | Done (M2) |
| `features/tenant` | Tenant | `/api/v1/tenants/current` | `modules/tenant` | Done (M2) |
| `features/profile` | Users + Tenant | `/users/{id}`, `/tenants/current`, JWT roles | `modules/users` | Done (M3) |
| `features/users` | Users | `/api/v1/users/*` | `modules/users` | Planned (list) |
| `features/files` | Files | `/api/v1/files` multipart | `shared/file-upload` | Done (M3) |
| `features/audit` | Audit | `/api/v1/audit-logs` | `modules/audit` | Done (M3) |
| `features/notifications` | Users prefs | `PATCH /users/{id}/preferences` | `NotificationPreferencesPage` | Done (M3) |
| `features/sessions` | Auth | `/api/v1/auth/sessions` | `SessionsPage` | Done (M3) |
| `features/settings` | Tenant | `/tenants/current/settings` | `TenantSettingsPage` | Done (M3) |
| `features/webhooks` | Webhooks | `/api/v1/webhooks/deliveries`, `/deadletters` | `modules/webhooks` (web admin) | Done (W5) |
| `features/apikeys` | ApiKeys | `/api/v1/api-keys` | `modules/apikeys` (web admin) | Done (V1) |

---

## Files on mobile (M3)

| Capability | API | Mobile UX |
|------------|-----|-----------|
| Upload | `POST /api/v1/files` | Camera, gallery, file picker |
| Download | `GET /api/v1/files/{id}` | Bytes + temp save |
| Delete | `DELETE /api/v1/files/{id}` | Confirm dialog |
| Preview | Auth stream | Image inline; PDF via OpenFilex |
| URL hint | `GET /api/v1/files/{id}/url` | Authenticated path only |

Session list in `FilesProvider` (no backend list endpoint).

---

## Routes (protected)

| Path | Feature |
|------|---------|
| `/home` | Module hub |
| `/profile` | Profile enhancements |
| `/files` | Files list + upload |
| `/files/{id}/preview` | Preview (full-screen) |
| `/notifications/preferences` | Email toggle |
| `/sessions` | Sessions list/revoke |
| `/settings` | Tenant settings |
| `/audit` | Audit viewer (Admin) |
| `/webhooks` | Webhook dashboard (`webhooks:read`) |
| `/webhooks/deliveries` | Delivery history |
| `/webhooks/deliveries/:id` | Delivery detail |
| `/webhooks/deadletters` | Dead letter list |
| `/webhooks/deadletters/:id` | Dead letter detail |
| `/api-keys` | API key list (`apikeys:read`) |
| `/api-keys/:id` | API key detail (read-only) |

---

## Release engineering (M5)

| Item | Path | Docs |
|------|------|------|
| Version source | `version.yaml` | [release/versioning/](./release/versioning/README.md) |
| Flavors | `tooling/android/`, `FlavorConfig` | [release/flavors/](./release/flavors/README.md) |
| Fastlane | `fastlane/` | [release/fastlane/](./release/fastlane/README.md) |
| Store assets | `store-assets/` | [release/store-readiness.md](./release/store-readiness.md) |

---

## Production capabilities (M4)

| Capability | Path | Docs |
|------------|------|------|
| Push | `core/notifications/` | [notifications/](./notifications/README.md) |
| Biometrics | `core/auth/biometrics/` | [biometrics/](./biometrics/README.md) |
| Deep links | `core/navigation/deep_links/` | [deep-links/](./deep-links/README.md) |
| Offline | `core/offline/` | [offline/](./offline/README.md) |
| Sync | `core/sync/` | [background-sync/](./background-sync/README.md) |
| Feature flags | `core/feature_flags/` | [feature-flags/](./feature-flags/README.md) |
| Crash reporting | `core/crash_reporting/` | [crash-reporting/](./crash-reporting/README.md) |
| Analytics | `core/analytics/` | [analytics/](./analytics/README.md) |

---

## Observer / cross-cutting

| Concern | Backend | Mobile `core/` |
|---------|---------|----------------|
| Audit writes | Domain events → Mongo | N/A (read-only viewer) |
| Correlation | Middleware | `X-Correlation-Id` + `ApiError` + banner |
| Feature flags | `Features` config | `core/feature-flags/` |
| Outbox | Hosted processors | N/A |

---

## Dependency direction

```
features/*  →  core/*  →  shared/*
```

Features must not reference `BackEnd` or `FrontEnd/apps/web`.

Module docs: [modules/](./modules/)
