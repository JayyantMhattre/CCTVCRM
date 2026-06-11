# Ashraak

Enterprise **multi-tenant SaaS starter template** — modular monolith API (.NET 10), React 19 SPA, and Flutter mobile foundation (`FrontEnd.Mobile/`).

Use Ashraak to bootstrap new products with Auth, Tenant isolation, User profiles, Audit (including login logging), and Redis caching already wired.

> **🧊 Platform V1 Complete & Frozen (`v1.0.0`).** The Core Platform is the stable, feature-complete baseline for all future products. Its scope is **frozen** — maintained (fixes, security, dependency upgrades, docs, new theme adapters) but not expanded with product-specific features. New product value is built as **business modules** on top. See the [Governance](#platform-governance) section and the [Platform Freeze Policy](docs/governance/platform-freeze-policy.md).

---

## Documentation

**Start here:** [docs/index.md](docs/index.md)

| Section | Link |
|---------|------|
| Getting started | [docs/getting-started/local-development.md](docs/getting-started/local-development.md) |
| Architecture | [docs/architecture/system-overview.md](docs/architecture/system-overview.md) |
| Module docs | [docs/architecture/module-map.md](docs/architecture/module-map.md) |
| API reference | [docs/api/overview.md](docs/api/overview.md) |
| Extension guides | [docs/extending/add-backend-module.md](docs/extending/add-backend-module.md) |
| Operations | [docs/operations/deployment-notes.md](docs/operations/deployment-notes.md) |
| **Platform V1 release** | [docs/releases/v1.0.0-release-notes.md](docs/releases/v1.0.0-release-notes.md) |
| **Governance (required)** | [docs/documentation-governance.md](docs/documentation-governance.md) |

Extended tutorial (rename, new module): [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md)

---

## Platform overview

| Layer | Technology |
|-------|------------|
| Backend | .NET 10, ASP.NET Core Minimal APIs, EF Core 9, OpenIddict |
| Web | React 19, TypeScript, Vite 6, TanStack Query, Zustand, CoreUI 5 |
| Mobile | Flutter (Android/iOS) — [FrontEnd.Mobile/](FrontEnd.Mobile/) · [docs/mobile/release/](docs/mobile/release/README.md) |
| Data | PostgreSQL (per-module schemas), Redis, MongoDB (audit) |
| Observability | Serilog + Seq, OpenTelemetry OTLP, correlation ID, health probes, Redis rate limits |

**Note:** Web is **React 19 + CoreUI**, not Angular. Mobile is **Flutter**, not native Kotlin/Swift ([ADR-0012](docs/adr/ADR-0012-flutter-mobile-platform.md)). Trace capabilities via [mobile platform manifest](docs/mobile/mobile-platform-manifest.md).

---

## Architecture (summary)

```
┌─────────────────────────────────────────────────────────┐
│  React SPA (FrontEnd/apps/web)                          │
└───────────────────────────┬─────────────────────────────┘
                            │ REST + OAuth2
┌───────────────────────────▼─────────────────────────────┐
│  Ashraak.Api (Host)                                     │
│  Caching → Auth → Tenant → Users → Audit (observer)     │
└───────────────────────────┬─────────────────────────────┘
         │              │              │
    PostgreSQL        Redis         MongoDB
    (auth,tenant,     (cache)       (audit)
     users)
```

Modules communicate via **SharedKernel.Contracts** only — no cross-module Infrastructure references.

Details: [docs/architecture/modular-monolith.md](docs/architecture/modular-monolith.md)

---

## Module map

| Module | Purpose |
|--------|---------|
| **Auth** | Identity, JWT, RBAC/ABAC, SSO (Google/Microsoft) |
| **Tenant** | Workspace provisioning, settings, plans |
| **Users** | Profiles and preferences (not credentials) |
| **Audit** | Login/API/entity/event capture → MongoDB |
| **Notifications** | Event-driven email (console/SMTP templates) |
| **Files** | Tenant-scoped storage (local / S3 / Azure) |
| **Caching** | Redis + memory cache, sessions, locks |
| **Outbox** | Hosted processors for Auth, Tenant, Users |
| **Webhooks** | Platform capability (W0 docs) — async event delivery for integrations |

Docs: [docs/modules/](docs/modules/) · Webhooks: [docs/modules/webhooks/](docs/modules/webhooks/README.md)

---

## Theme Engine

The web app is themed through a **Theme Adapter Architecture**: business modules never import a UI theme directly. Themes are swappable, coexist at runtime, and can be added without touching auth, routing, permissions, guards, APIs, or module logic. Direct theme integration is **forbidden** ([decision record](docs/frontend/themes/theme-decision-record.md)).

```
business module ──▶ platform-ui primitive ──▶ useTheme().adapter.<contract> ──▶ <theme>Adapter
                                                          ▲
                                            ThemeProvider (VITE_THEME → registry)
```

| Concept | What it is | Where |
|---------|------------|-------|
| **Theme Engine** | Stable `platform-ui` primitives (`PlatformCard`, `PlatformTable`, `PlatformDialog`, …) that modules render against — the only UI API modules use | `FrontEnd/apps/web/src/platform-ui/` |
| **Theme Adapter Architecture** | **11 typed contracts** (Layout, Navigation, Card, Table, Dialog, Notification, Badge, Avatar, Tabs, Breadcrumb, Chart) unified by `ThemeAdapter`; adapters reuse a theme's **visual design only** | `src/theme/contracts/`, `src/theme/adapters/<id>/` |
| **Theme Registry** | Maps a `ThemeId` to its adapter; adding a theme = one adapter folder + one registry line | `src/theme/registry.ts`, `src/theme/config.ts` |
| **Theme Switching** | Active theme resolved from `VITE_THEME` (safe default if unset/invalid) via `ThemeProvider` | `src/theme/ThemeProvider.tsx` |

**Available themes:** `coreui` (default, production) · `hexadash` (implemented, validated, opt-in). Switch with `VITE_THEME=hexadash`.

**Governance & onboarding (unlimited future themes):**

| Guide | Purpose |
|-------|---------|
| [theme-decision-record.md](docs/frontend/themes/theme-decision-record.md) | Why the adapter architecture; why direct integration is forbidden; benefits/tradeoffs |
| [theme-onboarding-guide.md](docs/frontend/themes/theme-onboarding-guide.md) | 6-step process to onboard a purchased theme (extract → analyse → compatibility → adapter → validate → activate) |
| [theme-lifecycle.md](docs/frontend/themes/theme-lifecycle.md) | Acquisition → analysis → implementation → validation → retirement |
| [theme-selection-checklist.md](docs/frontend/themes/theme-selection-checklist.md) | Pre-purchase checks (React version, TypeScript, Bootstrap, no Redux/routing/auth coupling) |
| [theme-adapter-development-guide.md](docs/frontend/themes/theme-adapter-development-guide.md) | How to implement each of the 11 contracts |
| [prompts/](docs/frontend/themes/prompts/) | Reusable analysis / compatibility / adapter / validation prompts |
| [current-theme/](docs/frontend/themes/current-theme/README.md) | CoreUI + HexaDash analysis, adapter, validation, parity & readiness reports |

**Theme Engine strategy:** UI is themed exclusively through swappable adapters; business code never imports a theme. Add unlimited future themes via the [onboarding guide](docs/frontend/themes/theme-onboarding-guide.md). New theme adapters are an allowed, additive change under the platform freeze.

---

## Platform Governance

Platform V1 is **frozen** at `v1.0.0`. Long-term governance keeps the Core Platform lean and reusable and routes new product value into business modules. These policies are **mandatory** for all future development.

| Policy | Purpose |
|--------|---------|
| [platform-freeze-policy.md](docs/governance/platform-freeze-policy.md) | Core Platform frozen after v1.0.0; what's allowed vs not; change requirements (justification + ADR + review + multi-project reuse) |
| [module-classification-policy.md](docs/governance/module-classification-policy.md) | Core Platform vs Business vs Experimental modules + approval process |
| [platform-extension-policy.md](docs/governance/platform-extension-policy.md) | The 8 criteria + decision matrix for entering Core |
| [business-module-policy.md](docs/governance/business-module-policy.md) | **Business module strategy** — optional, decoupled, replaceable, independently deployable; expected structure |
| [versioning-policy.md](docs/governance/versioning-policy.md) | SemVer for platform, themes, modules, mobile, backend + upgrade requirements |
| [architecture-decision-process.md](docs/governance/architecture-decision-process.md) | When ADRs are required + ADR workflow |
| [platform-lifecycle-policy.md](docs/governance/platform-lifecycle-policy.md) | Proposal → ADR → Review → Implementation → Documentation → Validation → Release + responsibilities |
| [platform-status-v1.md](docs/governance/platform-status-v1.md) | Completed / frozen / deferred capabilities + business module roadmap |

**Future platform extension process:** the default answer to "add this to Core?" is **no**. A capability enters Core only if it is used by multiple products, domain-independent, and clears the [extension policy](docs/governance/platform-extension-policy.md) gate (ADR + architecture review + docs + tests + operational owner). Otherwise it ships as a [business module](docs/governance/business-module-policy.md) or an experimental, flag-gated module.

**Release documentation:** [docs/releases/](docs/releases/v1.0.0-release-notes.md) — V1 release notes, capabilities, roadmap, known limitations, upgrade guide, architecture overview, and the V1 manifest.

---

## Quick start

### Prerequisites

- .NET SDK 10.0.103+
- Node.js 20+, pnpm 9+
- Docker Desktop

### 1. Infrastructure

```bash
cd BackEnd
Copy-Item .env.example .env
docker compose up -d
```

### 2. Backend

```bash
dotnet build Ashraak.slnx
dotnet run --project src/Host/Ashraak.Api
```

- API: http://localhost:5000  
- Scalar (dev): http://localhost:5000/scalar/v1  
- Health: http://localhost:5000/health/ready  

### 3. Frontend

```bash
cd FrontEnd
pnpm install
pnpm dev
```

- App: http://localhost:3000  

Full guide: [docs/getting-started/local-development.md](docs/getting-started/local-development.md)

---

## Environment variables

Backend: `BackEnd/.env.example` — see [docs/getting-started/environment-variables.md](docs/getting-started/environment-variables.md)

Frontend: `FrontEnd/apps/web/.env.development` — `VITE_API_BASE_URL`, `VITE_API_VERSION`, `VITE_APP_NAME`

---

## Debugging

| Layer | Guide |
|-------|-------|
| Backend | [docs/getting-started/debugging-guide.md](docs/getting-started/debugging-guide.md) |
| Frontend | Browser DevTools + Vite; session in `sessionStorage` key `ashraak_session` |
| Startup issues | [docs/operations/startup-troubleshooting.md](docs/operations/startup-troubleshooting.md) |

---

## Logging, Redis, Seq, OpenTelemetry

| Concern | Doc |
|---------|-----|
| Serilog | [docs/operations/logging.md](docs/operations/logging.md) |
| Seq UI | [docs/operations/seq-usage.md](docs/operations/seq-usage.md) — http://localhost:5341 |
| Redis | [docs/operations/redis-troubleshooting.md](docs/operations/redis-troubleshooting.md) |
| OpenTelemetry | [docs/architecture/observability.md](docs/architecture/observability.md) |

---

## Docker

```bash
cd BackEnd
docker compose up -d          # dev
# API in container: http://localhost:8080
```

Reference: [BackEnd/DOCKER_ENVIRONMENT.md](BackEnd/DOCKER_ENVIRONMENT.md)

---

## Extending the template

| Task | Guide |
|------|-------|
| New backend module | [docs/extending/add-backend-module.md](docs/extending/add-backend-module.md) |
| Contract events | [docs/extending/add-contracts-and-handlers.md](docs/extending/add-contracts-and-handlers.md) |
| Observer (like Audit) | [docs/extending/add-observer-module.md](docs/extending/add-observer-module.md) |
| Frontend route | [docs/extending/add-frontend-route.md](docs/extending/add-frontend-route.md) |

**Rule:** No feature without docs — [docs/documentation-governance.md](docs/documentation-governance.md)

---

## Documentation workflow

Code and documentation **evolve together** in the same pull request.

1. Implement the change  
2. Update canonical docs under [`docs/`](docs/index.md) (module folder + API/ops as needed)  
3. Add an [ADR](docs/adr/) for significant architectural decisions  
4. Run `.\scripts\validate-docs.ps1` (or `./scripts/validate-docs.sh`) before opening a PR  
5. Complete the [PR documentation checklist](docs/documentation-pr-checklist.md)

CI runs **Documentation Validation** on pull requests (warn by default; set repo variable `DOC_VALIDATE_MODE=fail` for a hard gate).

Guide: [docs/developer-workflow.md](docs/developer-workflow.md)

---

## Troubleshooting

- [Common failures](docs/errors/common-failures.md)
- [Error catalog](docs/errors/error-catalog.md)
- [Documentation audit](docs/documentation-audit/documentation-gap-analysis.md) — known scaffold vs implementation gaps

---

## Repository layout

```
Ashraak/
├── BackEnd/          # .NET solution
├── FrontEnd/         # React monorepo
├── docs/             # Canonical documentation
├── DEVELOPER_GUIDE.md
└── README.md
```

---

## License

Use according to your organization's template policy.
