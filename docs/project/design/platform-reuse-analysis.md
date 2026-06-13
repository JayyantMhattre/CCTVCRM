# Platform Reuse Analysis

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-5 — **most important document**: maps every frozen requirement to base-template reality.
**Platform baseline:** Ashraak Enterprise Platform V1 (frozen) — [platform discovery report](../../project-bootstrap/platform-discovery-report.md)

Classification per requirement area: **EXISTS** (already in base template — reuse as-is) · **EXTEND** (platform mechanism reused, CCTV wiring/config added) · **NEW MODULE** (CCTV business module required).
Effort scale: **S** (≤ 1 week) · **M** (1–3 weeks) · **L** (3–6 weeks) per area, single experienced developer, including module docs/tests per governance.

---

## 1. Requirement-by-requirement analysis

### Identity & access (freeze §3, §17, §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| Login, JWT, sessions, MFA | **EXISTS** | Platform Auth (OpenIddict); login screens web+mobile exist | — |
| Password Reset OTP / Login OTP | **EXISTS + EXTEND** | Auth flows exist; OTP delivery over **SMS** needs the SMS provider integration (below) | S |
| Roles Admin/Engineer/Customer | **EXTEND** | Role mechanism exists; add 2 role records + permission seeds ([permission-catalog.md](./permission-catalog.md)) | S |
| 30 CCTV permissions | **EXTEND** | `resource:action` machinery exists end-to-end (JWT claim → guards → API checks); CCTV adds permission definitions + seeds | S |
| Row-level scoping (own/assigned) | **NEW** (pattern reuse) | Same query-filter pattern as platform tenant scoping, applied per module | included in modules below |
| User account management | **EXISTS** | Platform Users module + admin screens | — |

### Files & media (freeze §12, §19, §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| Photo/video/selfie/signature storage | **EXISTS** | Platform Files: tenant-scoped, validated, scan hook, audited; web upload component + mobile camera/gallery/picker exist | — |
| FileId-only references | **EXISTS** (design rule) | Mandated in [database-architecture.md §7](./database-architecture.md) | — |
| PDF generation (Contract/Report/Invoice) | **NEW** | No platform PDF service — CCTV adds a server-side PDF renderer inside its modules, storing output via Files | M |

### Notifications (freeze §17, §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| Email channel + templates | **EXISTS** | Platform Notifications module (providers/templates, event-driven) | — |
| **SMS channel** | **EXTEND** | Platform has no SMS provider; add an SMS gateway integration consumed by CCTV notification flows (business-module scope — Core stays frozen) | M |
| 11 CCTV events | **EXTEND** | Wire domain events → notification dispatch (existing event-driven pattern) | S |
| Push to mobile | **EXISTS** | Mobile foundation push plumbing | — |
| Preference management | **EXISTS** | Platform user notification preferences (web+mobile screens) | — |

### Audit (freeze §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| Compliance audit trail | **EXISTS** | Audit observer auto-captures domain events → MongoDB hash chains; admin viewer web+mobile | — |
| CCTV auditable events | **EXTEND** | Raise domain events per [database-architecture.md §6](./database-architecture.md) — zero audit-module changes | included in modules |
| ⚠️ Audit read API | **EXISTS (stub)** | Platform GET endpoint is a stub — acceptable for V1 admin viewer scope; business histories are first-class CCTV tables anyway | — |

### Web shell & theme (freeze §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| SPA shell, routing, guards, layout, theming | **EXISTS** | React 19 + Theme Engine; CCTV renders `platform-ui` only | — |
| Admin Administration group | **EXISTS** | Users/Tenant/Audit/ApiKeys/Webhooks/Sessions screens mounted unchanged | — |
| 3 portal route trees + role redirect | **EXTEND** | New route groups on existing router/guards | S |

### Mobile (freeze §18, §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| Flutter foundation (auth, SDK, offline, sync, push, release CI) | **EXISTS** | Mandated reuse; release pipelines done | — |
| Customer App CCTV features | **NEW** (feature slices) | `features/cctv_*` consuming generated SDK (~14 NEW screens) | M |
| Engineer App CCTV features + offline capture | **NEW** (feature slices) | ~15 NEW screens; offline queue built on existing `core/offline` + `core/sync` | L |
| Two-app packaging (Customer vs Engineer) | **EXTEND** | Existing flavor/release tooling extended to role-targeted app builds | S |

### Integration surfaces (freeze §20)

| Requirement | Classification | Detail | Effort |
|-------------|----------------|--------|:------:|
| Outbound webhooks for CCTV events | **EXTEND** | Register CCTV event types in the existing catalog; publish via `IWebhookPublisher` | S |
| M2M API keys | **EXISTS** | Platform ApiKeys; CCTV permissions are valid scopes automatically | — |
| Anonymous inquiry endpoint protection | **EXISTS** | Platform rate limiting middleware | — |

### CCTV business functionality (freeze §4–§16) — **NEW MODULES**

| Module (schema) | Scope | Effort |
|-----------------|-------|:------:|
| Lead (`cctv_lead`) | Pipeline, auto-create from website, conversion orchestration | M |
| Customer/Site/Asset (`cctv_customer`) | Masters, ≤3 contacts, summary assets | M |
| AMC (`cctv_amc`) | Versioned plans, master+terms contracts, renewal, expiry reminders, contract PDF | L |
| Service (`cctv_service`) | Auto-generation, assignment, visit evidence, approval workflow, report PDF | **L** (largest) |
| Ticket (`cctv_ticket`) | Lifecycle, 3-actor creation, reopen, histories | M |
| Engineer (`cctv_engineer`) | Profiles, workload | S |
| Invoice (`cctv_invoice`) | Option B types, lines, lifecycle, invoice PDF | M |
| Reporting | Read-side views over modules | M |
| Public website | 9 pages + 2 forms (content reuse from www.aarvii.in) | M |
| Web portal UIs (Admin/Customer/Engineer ~58 NEW screens) | On platform shell/`platform-ui` | **L×2** |

## 2. Reuse scoreboard

| Area | EXISTS | EXTEND | NEW |
|------|:------:|:------:|:---:|
| Authentication & RBAC | ✅ core | roles+permissions seeds | row-scoping in modules |
| Files | ✅ all | — | PDF generation only |
| Notifications | ✅ email/push/prefs | SMS provider, event wiring | — |
| Audit | ✅ all | event raising | — |
| Theme/shell | ✅ all | route trees | screens (on shell) |
| Mobile infra | ✅ all | app packaging | feature slices |
| Webhooks/API keys | ✅ all | event catalog entries | — |
| Business domain | — | — | 7 modules + reporting + website |

**Duplicate-implementation check (mandated rules):** ❌ no duplicate auth · ❌ no duplicate file management · ❌ no duplicate notifications · ❌ no duplicate audit · ❌ no duplicate mobile infrastructure · ❌ no duplicate theme infrastructure. **All passed by design.**

## 3. Effort summary by area (indicative, not a commitment)

| Area | Effort band |
|------|-------------|
| Platform wiring (roles, permissions, route trees, webhook catalog, app packaging) | **S–M total** |
| SMS provider integration | M |
| PDF generation service (3 document types) | M |
| Backend business modules (7 + reporting) | ~5 × M + 2 × L |
| Web portals (Admin, Customer, Engineer + public site) | 2 × L + 2 × M |
| Mobile feature slices (2 apps) | M + L |
| **Relative split** | ≈ 15% platform wiring/extensions · 85% genuinely new business functionality |

The platform eliminates entire workstreams that would otherwise dominate: identity/MFA/sessions, file storage/security, email/push delivery, audit/compliance, theming, mobile foundation/release engineering, observability, CI — none of which appear in the NEW column.

## 4. Risks & dependencies

| Risk | Mitigation |
|------|------------|
| SMS provider selection (DEP-02, BRD) | Decide provider early in D2; abstract behind a CCTV-owned interface |
| PDF rendering choice | D0-6/D1 decision; store via Files regardless — no schema impact |
| Audit read API stub | CCTV in-app histories don't depend on it; platform viewer is admin-only convenience |
| No chart library committed | V1 dashboards use cards/tables ([dashboard-design.md](./dashboard-design.md)) |
| Two-app mobile packaging | Validate flavor-based approach against store policies during D6 planning |

---

Related: [permission-catalog.md](./permission-catalog.md) · [screen-inventory.md](./screen-inventory.md) · [mobile-screen-inventory.md](./mobile-screen-inventory.md) · [platform discovery report](../../project-bootstrap/platform-discovery-report.md)
