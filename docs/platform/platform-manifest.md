# Ashraak Platform Manifest

**Single source of truth** for V1 core platform capabilities.

Last updated: API Keys implementation — **Platform V1 Complete**

---

## V1 core capabilities

| Capability | Backend | Web | Mobile | Status |
|------------|---------|-----|--------|--------|
| Auth & MFA | ✓ | ✓ | ✓ | Done |
| Users & Roles | ✓ | ✓ | Partial | Done |
| Permissions (ABAC) | ✓ | ✓ | ✓ | Done |
| Sessions | ✓ | ✓ | ✓ | Done |
| Invitations | ✓ | Partial | Planned | Done |
| Tenants | ✓ | ✓ | ✓ | Done |
| Audit | ✓ | ✓ | ✓ | Done |
| Notifications | ✓ | ✓ | ✓ | Done |
| Files | ✓ | Partial | ✓ | Done |
| Feature Flags | ✓ | Partial | ✓ | Done |
| Webhooks | ✓ | ✓ | ✓ | Done |
| **API Keys** | ✓ | ✓ | ✓ | **Done** |
| Documentation governance | ✓ | ✓ | ✓ | Done |
| ADR governance | ✓ | — | — | Done |
| CI/CD governance | ✓ | ✓ | ✓ | Done |

---

## Platform V1 declaration

All core platform capabilities required for V1 are **implemented and documented**.

Future capabilities (CRM, LMS, HRMS, ERP, developer portals, partner onboarding) ship as **optional product modules** without expanding the platform core.

---

## Module manifests

- [Webhooks](../modules/webhooks/platform-manifest.md)
- [API Keys](../modules/apikeys/platform-manifest.md)
- [Mobile](../mobile/mobile-platform-manifest.md)
