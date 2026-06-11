# Module Classification Policy

**Status:** Mandatory · **Phase:** R1 — Platform Freeze & Governance

Every module in the codebase belongs to exactly **one** of three classes. Classification determines what governance applies, where the code lives, and how it is versioned and deployed.

**Related:** [platform-freeze-policy.md](./platform-freeze-policy.md) · [platform-extension-policy.md](./platform-extension-policy.md) · [business-module-policy.md](./business-module-policy.md) · [architecture-decision-process.md](./architecture-decision-process.md)

---

## The three classifications

| Class | Definition | Governance | Stability |
|-------|-----------|------------|-----------|
| **1. Core Platform** | Domain-independent capabilities reused by many products | **Frozen** at v1.0.0; changes need ADR + governance + multi-project justification | Highest |
| **2. Business Modules** | Product/domain-specific functionality built on the platform | Optional, decoupled, independently deployable | Per-product |
| **3. Experimental Modules** | Incubating ideas, not yet proven for Core or production | Sandbox; behind feature flags; explicitly unstable | Lowest |

---

## 1. Core Platform

**Criteria:** domain-independent, reusable across products, not tied to any business vertical. **Frozen** — see [platform-freeze-policy.md](./platform-freeze-policy.md).

**Members (V1):**

| Capability | Where |
|------------|-------|
| Auth | `Modules/Auth` |
| Users | `Modules/Users` |
| Roles | `Modules/Auth` (RBAC) |
| Permissions | `Modules/Auth` (ABAC/permissions) |
| MFA | `Modules/Auth` (TOTP) |
| Sessions | `Modules/Auth` (UserSession) |
| Invitations | `Modules/Auth` (Invitation aggregate) |
| Tenants | `Modules/Tenant` |
| Files | `Modules/Files` |
| Audit | `Modules/Audit` |
| Notifications | `Modules/Notifications` |
| Feature Flags | `Host` (ConfigFeatureFlagService) |
| Webhooks | `Modules/Webhooks` |
| API Keys | `Modules/ApiKeys` |
| Theme Engine | `apps/web/src/platform-ui` + `src/theme` |
| Mobile Platform | `FrontEnd.Mobile` foundation |
| Documentation Governance | `docs/` + validation CI |
| Release Engineering | `.github/workflows` + release ADRs |

> Note: Caching, Outbox, observability, and the Host composition root are also Core Platform infrastructure (foundational building blocks).

## 2. Business Modules

**Criteria:** product- or domain-specific; not universally reusable; encodes business workflows. Must be optional, decoupled, replaceable, and independently deployable — see [business-module-policy.md](./business-module-policy.md). **None ship in V1**; the platform is the foundation for them.

**Examples:**

- CRM
- Lead Management
- Billing
- Subscriptions
- LMS (Learning Management)
- HRMS
- ERP
- Project Management
- Knowledge Base
- Helpdesk / Ticketing

## 3. Experimental Modules

**Criteria:** unproven, exploratory; may graduate to Core (via the [extension policy](./platform-extension-policy.md)) or to a business module, or be retired. Always behind feature flags; never a hard dependency of Core.

**Examples:**

- AI Agents
- Workflow Engine
- RAG (retrieval-augmented generation)
- Automation Framework
- Future Integrations

---

## Classifying a new capability — quick test

```
Is it domain-independent AND reused by multiple products?
   ├─ YES → candidate for CORE PLATFORM → run platform-extension-policy.md gate (ADR required)
   └─ NO  → Is it product/domain-specific business value?
              ├─ YES → BUSINESS MODULE (business-module-policy.md)
              └─ NO / unproven exploration → EXPERIMENTAL MODULE (flagged sandbox)
```

When in doubt, default to **Business Module** or **Experimental** — never Core. Core membership is the highest bar and is frozen.

---

## Approval process by class

| Class | Who approves | Required artifacts | Entry path |
|-------|--------------|--------------------|------------|
| **Core Platform** | Platform governance review | Documented justification + **ADR** + multi-project reuse evidence + architecture review + docs + tests + operational owner | [platform-extension-policy.md](./platform-extension-policy.md) → [platform-lifecycle-policy.md](./platform-lifecycle-policy.md) |
| **Business Module** | Product owner + tech lead | Module spec + docs; conforms to [business-module-policy.md](./business-module-policy.md) structure | Standard module workflow ([extending](../extending/add-backend-module.md)) |
| **Experimental Module** | Tech lead | Lightweight proposal + feature flag + "experimental" label | Sandbox; ADR only if it later seeks Core/business graduation |

**Graduation:** an Experimental module that proves multi-product value can be promoted to Core (full extension gate) or hardened into a Business Module. Promotion is never automatic.

---

## Rules

1. A module declares its classification in its README.
2. Core never depends on Business or Experimental modules (dependencies point downward: Business/Experimental → Core, never the reverse).
3. Business and Experimental modules communicate with Core only through published contracts/APIs.
4. Reclassification (e.g., Experimental → Core) requires the full Core entry path.
