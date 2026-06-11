# Mobile platform manifest

**Single source of truth** for Backend ↔ Web ↔ Mobile coverage.

Last updated: V1 (API Keys — platform complete)

---

## Legend

| Symbol | Meaning |
|--------|---------|
| ✓ | Implemented and documented |
| Partial | Backend/web exists; gaps noted |
| Planned | Documented; not implemented on client |
| N/A | Not applicable on client |

**Mobile status values:** `Planned` | `In Progress` | `Done` | `N/A`

---

## Platform modules

| Module / Capability | Backend | Web | Mobile | Mobile status | Notes |
|---------------------|---------|-----|--------|---------------|-------|
| **Auth** | ✓ | ✓ | ✓ | Done | |
| **MFA** | ✓ | Partial | ✓ | Done | |
| **Sessions** | ✓ | ✓ | ✓ | Done | |
| **Invitations** | ✓ | Partial | Planned | M6 | Deep link scaffold |
| **Tenant** | ✓ | ✓ | ✓ | Done | |
| **Tenant settings** | ✓ | ✓ | ✓ | Done | |
| **Users** (profile) | ✓ | ✓ | Partial | In Progress | List planned |
| **User preferences** | ✓ | ✓ | ✓ | Done | |
| **Notifications** | ✓ | ✓ | ✓ | Done | |
| **Audit** | ✓ | ✓ | ✓ | Done | |
| **Files** | ✓ | Partial | ✓ | Done | |
| **Dashboard** | N/A | ✓ | ✓ | Done | |
| **Correlation ID** | ✓ | ✓ | ✓ | Done | |
| **Feature flags** | ✓ | Partial | ✓ | Done | |
| **Health / observability** | ✓ | N/A | Partial | In Progress | Crash/analytics abstraction |
| **Webhooks** | Done | Done | Done | Done | W5 read-only operational visibility |
| **API Keys** | Done | Done | Done | Done | V1 M2M auth + read-only mobile |

---

## Platform V1

**Status: Complete.** All core platform capabilities implemented. Future work ships as optional product modules.

---

## Infrastructure (mobile-specific)

| Capability | Backend | Web | Mobile | Mobile status |
|------------|---------|-----|--------|---------------|
| **Flutter foundation** | — | — | ✓ | Done |
| **Production capabilities** (M4) | — | — | ✓ | Done |
| **Release engineering** | — | — | ✓ | Done |
| **Store readiness** | — | — | ✓ | Done |
| **Build flavors** (dev/qa/uat/prod) | — | — | ✓ | Done |
| **Versioning automation** | — | — | ✓ | Done |
| **Android signing (CI + local)** | — | — | ✓ | Done |
| **iOS signing strategy** | — | — | ✓ | Done |
| **Fastlane** | — | — | ✓ | Done |
| **Release CI** (APK/AAB/IPA) | — | — | ✓ | Done |
| **OpenAPI / SDK generation** | Partial | N/A | Partial | In Progress |
| **CI** (analyze, test, flavors) | — | — | ✓ | Done |

---

## Update checklist (per PR)

- [ ] Row added or status changed in this manifest
- [ ] `docs/mobile/module-map.md` aligned
- [ ] Release docs if flavor/signing changed
