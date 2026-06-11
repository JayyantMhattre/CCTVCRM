# Business Module Policy

**Status:** Mandatory · **Phase:** R1 — Platform Freeze & Governance

Defines how product- and domain-specific functionality is built **on top of** the frozen Core Platform. Business modules are where new product value lives after v1.0.0.

**Related:** [module-classification-policy.md](./module-classification-policy.md) · [platform-extension-policy.md](./platform-extension-policy.md) · [platform-freeze-policy.md](./platform-freeze-policy.md) · [extending guides](../extending/add-backend-module.md)

---

## 1. The four mandatory properties

Every business module **must** be:

| Property | Meaning | Why |
|----------|---------|-----|
| **Optional** | The platform builds and runs without it; removing it never breaks Core | Keeps the platform lean; products pick only what they need |
| **Decoupled** | Talks to Core only through published contracts/APIs; no edits to Core code | Protects the freeze; modules evolve independently |
| **Replaceable** | Can be swapped for an alternative implementation without touching Core | Avoids lock-in; competing implementations possible |
| **Independently deployable** | Can be built, versioned, and released on its own cadence | Product teams move at their own speed |

A module that cannot satisfy all four is mis-scoped — it likely belongs in Core (rare, via [extension policy](./platform-extension-policy.md)) or should be redesigned.

## 2. Examples of business modules

- CRM
- Lead Management
- Billing
- LMS (Learning Management)
- HRMS
- ERP
- Ticketing / Helpdesk
- AI Solutions
- Project Management
- Knowledge Base

None ship in V1; the platform exists to host them.

## 3. Relationship to Core (dependency direction)

```
Business Module ──(consumes contracts/APIs)──▶ Core Platform
        ▲                                            │
        └──────────── never the reverse ─────────────┘
```

- Core **never** depends on, references, or is aware of any business module.
- Business modules consume Core capabilities (Auth, Tenants, Permissions, Files, Notifications, Webhooks, API Keys, Audit, Theme Engine, etc.) through their published surfaces.
- Business modules **must not** modify Core code to function. Needed Core changes go through the [extension policy](./platform-extension-policy.md).

## 4. Expected structure

Business modules mirror the platform's vertical-slice conventions so they feel native and stay decoupled.

### Backend business module
```
Modules/<BusinessModule>/
├── <Name>.Domain/          # aggregates, value objects, domain events
├── <Name>.Application/     # commands, queries, handlers, abstractions
├── <Name>.Infrastructure/  # EF Core, repositories, outbox writer, integrations
└── <Name>.Api/             # Minimal API endpoints, registered via a module extension
```
Rules: own database schema; integrate via `SharedKernel.Contracts` and domain events; register through a module registration extension in the Host; no cross-module Infrastructure references. Follow [add-backend-module](../extending/add-backend-module.md).

### Web business module
```
apps/web/src/modules/<businessModule>/
├── pages/        # route components
├── components/   # module-local components (render platform-ui primitives)
├── api/          # typed API calls (TanStack Query)
├── hooks/        # module hooks
├── guards/       # route guards (reuse Core guards)
└── types.ts      # API/response interfaces
```
Rules: render **`platform-ui`** primitives (never import a theme directly); reuse Core auth/routing/guards; mount via the central router. Follow [add-frontend-route](../extending/add-frontend-route.md).

### Mobile business module
```
FrontEnd.Mobile/lib/features/<feature>/
├── pages/        # screens
├── providers/    # Riverpod providers
├── data/         # repositories
├── models/       # models
└── widgets/      # feature widgets
```
Rules: use Riverpod + go_router; consume the generated OpenAPI SDK; no edits to Core foundation.

## 5. Required artifacts

| Artifact | Requirement |
|----------|-------------|
| Module README | Declares classification = Business Module, owner, and Core dependencies |
| Documentation | `docs/modules/<module>/` (+ API/ops as needed) per [documentation-governance](../documentation-governance.md) |
| Tests | Automated coverage for the module |
| Versioning | Independent SemVer per [versioning-policy.md](./versioning-policy.md) |
| Feature flag (recommended) | Gate rollout where appropriate ([feature-flags](../platform/feature-flags/README.md)) |

## 6. Anti-patterns (rejected in review)

- ❌ Importing or editing Core internals instead of using contracts.
- ❌ Making Core depend on the module.
- ❌ Importing a UI theme directly in a web module (must use `platform-ui`).
- ❌ Sharing a database schema with Core or another module.
- ❌ Coupling two business modules so neither is independently deployable.

---

**Summary:** Business modules are optional, decoupled, replaceable, independently deployable add-ons that consume the frozen Core Platform through contracts. This is the sanctioned path for all post-V1 product development.
