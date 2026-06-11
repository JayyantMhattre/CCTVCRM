# Documentation Governance

**Effective:** May 2026  
**Applies to:** All contributors, all phases, all modules (backend, web, and mobile).

---

## Mandatory rule

> **No new feature may be implemented without documentation.**

Documentation quality must match code quality. Treat docs as part of the definition of done.

---

## Core principles

### 1. Code is source of truth

- Audit the implementation before writing or updating docs.
- If documentation and code disagree, **update documentation** to match code.
- Do not change production code solely to match outdated documentation (unless the code is objectively wrong and tracked as a bug).

### 2. Documentation is not optional

Every pull request that changes behavior must include documentation updates in the same change set (or a immediately linked follow-up PR with explicit approval — discouraged).

### 3. Module documentation is mandatory

Any module — **existing or new** — must maintain the standard layout under `/docs/modules/{module-name}/`:

| File | Purpose |
|------|---------|
| `README.md` | Overview, status, navigation |
| `architecture.md` | Design within the monolith |
| `registration.md` | DI and host wiring |
| `api.md` | HTTP surface (or N/A stated explicitly) |
| `events.md` | Domain and contract events |
| `extending.md` | Safe extension points |
| `operations.md` | Configuration, health, troubleshooting |

### 4. Mobile platform documentation (M0+)

Flutter mobile is a **first-class platform**. Before any `FrontEnd.Mobile/` code:

- Maintain `/docs/mobile/` per [mobile-governance.md](./mobile/mobile-governance.md)
- Update [mobile-platform-manifest.md](./mobile/mobile-platform-manifest.md) when backend/web/mobile coverage changes
- Use `ADR-Mobile-*` for client architecture decisions

### 5. Webhook platform documentation (W0+)

Webhooks are a **platform capability**, not a vertical business module. Before any webhook implementation:

- Maintain `/docs/modules/webhooks/` per [governance.md](./modules/webhooks/governance.md)
- Update [platform-manifest.md](./modules/webhooks/platform-manifest.md) on every capability change
- Use `ADR-Webhook-*` for webhook architecture decisions
- Register new event types in [event-catalog.md](./modules/webhooks/event-catalog.md) before publishers ship

### 6. Docs updated with code

When you change:

- Endpoints → update `api.md` and `/docs/api/`
- Events → update `events.md` for publisher and all consumers
- Middleware order → update `docs/modules/host/`
- Environment variables → update `getting-started/environment-variables.md`
- Architecture boundaries → update `docs/architecture/` and consider a new ADR

### 7. Hub files maintained

- `/docs/index.md` — add links for every new doc area
- Root `README.md` — update setup steps when toolchain or ports change

### 8. Architecture docs for design changes

Structural changes (new module, new integration, new datastore) require:

- Update to `docs/architecture/module-map.md`
- New ADR in `docs/adr/` when the decision is significant or irreversible

---

## PR checklist (copy into PR description)

```markdown
## Documentation
- [ ] Code audited — docs match implementation
- [ ] Module docs updated (`/docs/modules/...`) if module touched
- [ ] API docs updated if endpoints changed
- [ ] Getting-started / env vars updated if setup changed
- [ ] ADR added if architectural decision made
- [ ] `/docs/index.md` links added for new pages
- [ ] Stubs/limitations labeled honestly (no aspirational docs)
```

---

## What to document for new features

| Change type | Minimum documentation |
|-------------|-------------------------|
| New backend module | Full 7-file module set + register in `module-map.md` + `extending/add-backend-module.md` example |
| New endpoint | Module `api.md` + OpenAPI metadata in code (`WithSummary`, `WithDescription`) |
| New contract | `SharedKernel.Contracts` entry in `shared-kernel/events.md` + consumer module `events.md` |
| New observer (audit, etc.) | Observer module `events.md` + publisher `events.md` |
| New frontend page/route | `frontend/routing-and-guards.md` + feature section |
| New env var | `environment-variables.md` + module `operations.md` |
| Breaking change | ADR + migration notes in getting-started |

---

## Style and quality bar

**Do:**

- Use concrete file paths (`BackEnd/src/Host/Ashraak.Api/Program.cs`)
- State HTTP methods and full route patterns (`POST /api/v1/auth/register`)
- Label implementation status: **Implemented**, **Stub**, **Scaffold**, **Not wired**
- Link related docs instead of duplicating large sections

**Do not:**

- Document features that are not in the codebase
- Use generic filler without implementation references
- Assume Angular — the frontend is **React 19 + Vite 6**
- Copy outdated content from legacy `*_MODULE_STATUS.md` without verification

---

## Legacy documentation

Files outside `/docs/` (e.g. `DEVELOPER_GUIDE.md`, `*_MODULE_STATUS.md`) may remain during transition.

**Canonical entry:** `/docs/modules/{name}/README.md`  
**Rule:** When legacy and `/docs` conflict, `/docs` wins after audit.

---

## Audit cadence

- **Per major release:** Run documentation audit (gap + outdated reports).
- **Per new module:** Verify seven-file completeness before merge.
- **Quarterly:** Review ADRs for superseded decisions.

---

## Enforcement

### Automated validation

| Asset | Purpose |
|-------|---------|
| [doc-validation.json](./doc-validation.json) | Code path → doc path mappings |
| [scripts/validate-docs.ps1](../scripts/validate-docs.ps1) | Local check (Windows) |
| [scripts/validate-docs.sh](../scripts/validate-docs.sh) | Local / CI check (bash) |
| [.github/workflows/docs-validation.yml](../.github/workflows/docs-validation.yml) | PR/push workflow |

**Default mode:** `warn` — prints missing doc updates, does not fail build.

**CI:** GitHub Actions workflow `ci.yml` runs package audit, `dotnet restore/build/test`, then docs validation on every PR/push to `main`/`master`. Set repository variable `DOC_VALIDATE_MODE=fail` to enforce docs on merge.  
**Strict mode:** set repository variable `DOC_VALIDATE_MODE=fail` or run scripts with `-Mode fail`.

### Team process

1. CODEOWNERS on `/docs/` for architect review
2. PR checklist: [documentation-pr-checklist.md](./documentation-pr-checklist.md)
3. Workflow guide: [developer-workflow.md](./developer-workflow.md)
4. AI-assisted onboarding must use `/docs/index.md` as entry point

---

## Related

- [Documentation audit findings](./documentation-audit/documentation-gap-analysis.md)
- [Standardization plan](./documentation-audit/documentation-standardization-plan.md)
- [Documentation index](./index.md)
