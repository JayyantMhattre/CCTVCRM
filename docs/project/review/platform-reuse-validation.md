# Platform Reuse Validation

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-0 — **Most important validation document**  
**Rule:** Verify every requirement is classified Reuse / Extend / New — **no duplicate platform functionality**

**References:** [platform-reuse-analysis.md](../design/platform-reuse-analysis.md) · [platform-reuse-roadmap.md](../roadmap/platform-reuse-roadmap.md) · [platform-v1-manifest.md](../../releases/platform-v1-manifest.md)

---

## 1. Validation verdict

| Check | Result |
|-------|:------:|
| Duplicate Auth implementation scheduled | ❌ None — **PASS** |
| Duplicate Files implementation scheduled | ❌ None — **PASS** |
| Duplicate Notifications core scheduled | ❌ None — **PASS** |
| Duplicate Audit observer scheduled | ❌ None — **PASS** |
| Duplicate Theme Engine scheduled | ❌ None — **PASS** |
| Duplicate Mobile foundation scheduled | ❌ None — **PASS** |
| Duplicate Webhooks engine scheduled | ❌ None — **PASS** |
| Duplicate CI/CD scheduled | ❌ None — **PASS** |
| All freeze §20 capabilities mapped | ✅ **PASS** |
| CCTV builds business modules only | ✅ **PASS** |

**Platform reuse validation: APPROVED**

---

## 2. Requirement-by-requirement classification

Legend: **REUSE** = platform capability as-is · **EXTEND** = platform mechanism + CCTV config/wiring · **NEW** = CCTV business module or feature slice

### 2.1 Authentication & sessions (freeze §3, §17, §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Login, JWT, refresh | **REUSE** | Auth module, OpenIddict | Wire Customer/Engineer portal routes | ❌ |
| MFA | **REUSE** | Auth MFA | Enable for Admin; optional others | ❌ |
| Sessions management | **REUSE** | Auth sessions | Admin screen #44 REUSE | ❌ |
| Password reset | **REUSE** | Auth flows | Customer portal screen #57 | ❌ |
| Password Reset OTP | **REUSE + EXTEND** | Auth OTP mechanism | SMS delivery via EXTEND (G-R02) | ❌ |
| Login OTP | **REUSE + EXTEND** | Auth OTP mechanism | SMS delivery via EXTEND | ❌ |
| User account CRUD | **REUSE** | Users module | Admin screens #39–40 REUSE | ❌ |

### 2.2 Authorization & RBAC (freeze §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Permission machinery | **REUSE** | JWT claims, PermissionGuard, IAuthPermissionChecker | Zero platform code change | ❌ |
| Admin role | **REUSE** | Existing Admin role | Grant 30 CCTV permissions | ❌ |
| Engineer role | **EXTEND** | Role store | New role record + seeds | ❌ |
| Customer role | **EXTEND** | Role store | New role record + seeds | ❌ |
| 30 CCTV permissions | **EXTEND** | Permission seed pattern | D1 migration | ❌ |
| Row-level scoping | **NEW** (pattern reuse) | Tenant filter pattern | Per-module query filters | ❌ |

### 2.3 Files & media (freeze §12, §19, §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Photo/video/selfie/signature storage | **REUSE** | Files module | Two-step upload + link endpoints | ❌ |
| FileId-only references | **REUSE** (design rule) | Files API | No paths in CCTV tables | ❌ |
| Web FileUpload component | **REUSE** | shared/file-upload | All CCTV upload UX | ❌ |
| Mobile camera/gallery | **REUSE** | Mobile files feature | Engineer/customer apps | ❌ |
| PDF binary storage | **REUSE** | Files module | PDF service uploads output | ❌ |
| PDF **generation** | **NEW** | Not in platform | CCTV-owned renderer (B3/B4/B6) | ❌ |

### 2.4 Notifications (freeze §17, §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Email channel | **REUSE** | Notifications module | CCTV templates + handlers | ❌ |
| Push (mobile) | **REUSE** | Mobile push + Notifications | Event wiring | ❌ |
| Preference management | **REUSE** | Platform preferences UI | Customer screen #58 REUSE pattern | ❌ |
| SMS channel | **EXTEND** | No SMS provider in platform | `ISmsProvider` in CCTV scope (D1) | ❌ |
| 11 CCTV domain events | **EXTEND** | Event-driven dispatch | Handlers in business modules | ❌ |

### 2.5 Audit (freeze §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Compliance capture | **REUSE** | Audit observer → MongoDB | Register CCTV DbContexts | ❌ |
| Admin audit viewer | **REUSE** | Frontend audit-viewer #41 | `audit:read` permission | ❌ |
| Business history tables | **NEW** | N/A | TicketStatusHistory, etc. | ❌ |
| Audit read API (full query) | **REUSE (stub)** | Platform Phase 2 | Accept stub for V1 | ❌ |

### 2.6 Webhooks & API keys (freeze §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Outbound webhooks | **REUSE** | Webhooks engine | Catalog entries + IWebhookPublisher | ❌ |
| M2M API keys | **REUSE** | ApiKeys module | Admin screen #42 REUSE | ❌ |
| Webhook admin UI | **REUSE** | Webhooks admin | Admin screen #43 REUSE | ❌ |
| Rate limiting (inquiry) | **REUSE** | Platform middleware | Public inquiry endpoint | ❌ |

### 2.7 Theme engine & web shell (freeze §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| SPA shell, layout, router | **REUSE** | React 19 + CoreUI default | CCTV routes on existing shell | ❌ |
| AuthGuard, RoleGuard, PermissionGuard | **REUSE** | core/router | Portal route trees | ❌ |
| platform-ui primitives | **REUSE** | 11 theme contracts | All 58 NEW CCTV screens | ❌ |
| Theme adapters (CoreUI/HexaDash) | **REUSE** | theme/adapters | No CCTV theme fork | ❌ |
| Admin Administration group | **REUSE** | Existing nav | Users/Tenant/Audit/ApiKeys/Webhooks | ❌ |
| 3 portal route trees | **EXTEND** | Router config | D1 nav skeleton | ❌ |

**Theme rule validated:** CCTV imports `@/platform-ui` only — no direct `@coreui` or adapter imports ([theme-usage-design.md](../design/lld/theme-usage-design.md)).

### 2.8 Mobile foundation (freeze §18, §20)

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| Flutter app structure | **REUSE** | FrontEnd.Mobile | Feature slices only | ❌ |
| Auth, secure storage | **REUSE** | mobile/auth | Customer + Engineer apps | ❌ |
| OpenAPI SDK generation | **REUSE** | packages/api_client | Regen with CCTV routes | ❌ |
| Offline cache + sync | **REUSE** | core/offline, core/sync | Engineer visit queue | ❌ |
| Push notifications | **REUSE** | mobile/notifications | CCTV event handlers | ❌ |
| Release CI (fastlane) | **REUSE** | mobile.yml, store pipelines | Two-app flavors | ❌ |
| Customer CCTV features | **NEW** | N/A | ~14 screens | ❌ |
| Engineer CCTV features | **NEW** | N/A | ~15 screens + offline | ❌ |
| Two-app packaging | **EXTEND** | Flavor tooling | Customer vs Engineer builds | ❌ |

### 2.9 Release engineering & governance

| Requirement | Classification | Platform source | CCTV action | Duplicate? |
|-------------|----------------|-----------------|-------------|:----------:|
| CI/CD pipelines | **REUSE** | 5 GitHub workflows | Extend for CCTV test projects | ❌ |
| Documentation governance | **REUSE** | 7-file module docs | Apply to cctv-* modules | ❌ |
| ADR process | **REUSE** | docs/adr/ | CCTV ADRs in docs/project/adr/ | ❌ |
| Architecture tests | **REUSE** | BuildingBlocks tests | Add CCTV boundary rules | ❌ |

---

## 3. CCTV business modules — all NEW (correct)

| Module | Schema | Classification | Platform overlap |
|--------|--------|----------------|------------------|
| Lead Management | `cctv_lead` | **NEW** | None |
| Customer/Site/Asset | `cctv_customer` | **NEW** | None |
| AMC Plans/Contracts | `cctv_amc` | **NEW** | None |
| Service Scheduling/Visits | `cctv_service` | **NEW** | None |
| Ticket Management | `cctv_ticket` | **NEW** | None |
| Engineer Management | `cctv_engineer` | **NEW** | None |
| Invoice Management | `cctv_invoice` | **NEW** | None |
| Reporting | (read-only) | **NEW** | None |
| Public Website (business pages) | N/A | **NEW** | Uses platform shell |
| Customer Portal (business screens) | N/A | **NEW** | Uses platform shell |
| Engineer Portal (business screens) | N/A | **NEW** | Uses platform shell |

---

## 4. Duplicate functionality flags

| Flag | Finding | Action |
|------|---------|--------|
| F-01 | No second auth module planned | ✅ Clear |
| F-02 | No CCTV file storage module planned | ✅ Clear |
| F-03 | No CCTV notification dispatcher planned | ✅ Clear — handlers only |
| F-04 | No CCTV audit storage planned | ✅ Clear — observer only |
| F-05 | No CCTV theme fork planned | ✅ Clear |
| F-06 | No custom mobile HTTP client planned | ✅ Clear — OpenAPI SDK |
| F-07 | PDF service in CCTV scope | ✅ **Not a duplicate** — platform has no PDF |
| F-08 | SMS adapter in CCTV scope | ✅ **Not a duplicate** — platform has no SMS |

**No duplicate platform functionality identified.**

---

## 5. Effort split validation

| Source | Platform wiring | New business |
|--------|:---------------:|:------------:|
| platform-reuse-analysis | ~15% | ~85% |
| platform-reuse-roadmap | ~15% | ~85% |
| implementation-roadmap | Platform = 0 sprints | B1–B7 + FP + M |

**Consistent across D0-5 and D0-8.**

---

## 6. Platform manifest cross-check

| Manifest item | Count | CCTV usage |
|---------------|------:|------------|
| Backend modules | 9 | All REUSE — no 10th platform module |
| Web modules | 7 | All REUSE for admin platform screens |
| Mobile features | 11 | All REUSE for infra |
| Theme adapters | 2 | REUSE — CoreUI default |
| ADRs | 30 | Follow for extensions |
| CI pipelines | 5 | REUSE + extend |

---

## 7. Conclusion

Every frozen requirement maps to **REUSE**, **EXTEND**, or **NEW business module**. Platform V1 eliminates identity, files, email/push, audit capture, theming, mobile infra, webhooks, and CI workstreams. CCTV implementation correctly schedules **only business domain modules + minimal platform wiring**.

**Platform reuse validation: APPROVED for D1-1.**

---

Related: [architecture-validation-report.md](./architecture-validation-report.md) · [gap-analysis-report.md](./gap-analysis-report.md)
