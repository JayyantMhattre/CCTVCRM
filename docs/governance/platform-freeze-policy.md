# Platform Freeze Policy

**Status:** Mandatory · **Effective:** Platform V1 (`v1.0.0`) · **Phase:** R1 — Platform Freeze & Governance
**Applies to:** the Core Platform (see [module-classification-policy.md](./module-classification-policy.md)).

> This policy is binding for all future development. It exists to protect the Core Platform as a stable, reusable baseline and to prevent platform bloat.

**Related:** [platform-extension-policy.md](./platform-extension-policy.md) · [architecture-decision-process.md](./architecture-decision-process.md) · [versioning-policy.md](./versioning-policy.md) · [V1 release notes](../releases/v1.0.0-release-notes.md)

---

## 1. Declaration

**Ashraak Platform V1 (`v1.0.0`) is the frozen baseline release.** As of this version, the Core Platform is considered **stable and feature-complete**. It is the foundation every downstream product and module builds on.

"Frozen" does **not** mean "unmaintained." It means the Core Platform's **feature scope is closed**: it is maintained (fixed, secured, upgraded, documented) but not expanded with new product- or domain-specific functionality.

## 2. What the freeze protects

The freeze applies to the **Core Platform** capabilities catalogued in [platform-status-v1.md](./platform-status-v1.md) and [module-classification-policy.md](./module-classification-policy.md) — Auth, Users, Roles, Permissions, MFA, Sessions, Invitations, Tenants, Files, Audit, Notifications, Feature Flags, Webhooks, API Keys, Theme Engine, Mobile Platform, Documentation Governance, and Release Engineering.

## 3. Changes that require governance

Any change to a frozen Core Platform capability requires **all** of the following before implementation:

1. **Documented justification** — a written problem statement and why it belongs in the platform (not a business module).
2. **ADR** — an Architecture Decision Record per [architecture-decision-process.md](./architecture-decision-process.md).
3. **Governance review** — approval through the [platform lifecycle](./platform-lifecycle-policy.md).
4. **Multi-project reuse justification** — evidence the change benefits **multiple products**, not one (the platform-extension bar in [platform-extension-policy.md](./platform-extension-policy.md)).

A change failing any of these is rejected from Core, or redirected to a business/experimental module.

## 4. Allowed without scope expansion

These keep the platform healthy and **do not** count as feature expansion (they still follow normal review + docs + tests):

| Allowed | Notes |
|---------|-------|
| **Bug fixes** | Correct defects in existing behavior; no new surface. |
| **Security fixes** | Patch vulnerabilities; expedited path permitted (see §6). |
| **Dependency upgrades** | Per [upgrade-guide](../releases/upgrade-guide.md): one major at a time, build verified, docs updated. |
| **Documentation improvements** | Always welcome; governed by [documentation-governance](../documentation-governance.md). |
| **Theme adapter additions** | New themes via the [theme onboarding guide](../frontend/themes/theme-onboarding-guide.md). Additive, isolated, and explicitly allowed by the freeze. |

## 5. Not allowed in Core Platform

Domain- and product-specific functionality **must not** enter the Core Platform. It belongs in **Business Modules** ([business-module-policy.md](./business-module-policy.md)):

- ❌ Domain-specific features
- ❌ Product-specific workflows
- ❌ CRM features
- ❌ Billing / subscription features
- ❌ Lead management features
- ❌ LMS features
- ❌ HRMS / ERP features
- ❌ Any single-product capability

If a capability is requested for Core and is business-specific, the correct outcome is a **business module**, not a platform change.

## 6. Exception path (security)

Critical security fixes may be implemented on an expedited timeline. They still require an ADR and documentation, which **may be completed retroactively within the same release cycle** — but never skipped.

## 7. Enforcement

- Pull requests touching Core Platform code without the §3 artifacts are **not merged**.
- The [documentation validation CI](../../.github/workflows/docs-validation.yml) and PR review enforce the docs/ADR requirement.
- Disagreements about Core-vs-business classification are resolved using the [decision matrix](./platform-extension-policy.md#decision-matrix).

---

**Outcome:** Core Platform frozen at `v1.0.0`. Maintained, not expanded. New product value is delivered as business modules on top of the frozen platform.
