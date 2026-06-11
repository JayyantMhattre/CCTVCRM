# Documentation Standardization Plan

**Status:** Implemented (this phase)  
**Goal:** Single navigable documentation system with governance for all future work.

---

## Target structure

```
docs/
├── index.md                          # Master hub
├── documentation-governance.md       # Mandatory rules
├── documentation-audit/              # This audit (frozen snapshot)
├── architecture/                     # System design
├── adr/                              # Architecture decisions
├── modules/                          # Per-module standard docs
│   ├── shared-kernel/
│   ├── auth/
│   ├── tenant/
│   ├── users/
│   ├── audit/
│   ├── caching/
│   ├── host/
│   └── building-blocks/
├── frontend/                         # React SPA docs
├── api/                              # HTTP API reference
├── getting-started/                  # Onboarding
├── extending/                        # How-to guides
├── operations/                       # Runbooks
└── errors/                           # Error catalog
```

---

## Module documentation standard

Every backend module (existing or future) **must** include:

```
docs/modules/{module-name}/
├── README.md           # Purpose, status, quick links
├── architecture.md     # Layers, aggregates, storage
├── registration.md     # DI + endpoint wiring
├── api.md              # HTTP surface (if applicable)
├── events.md           # Domain + contract events
├── extending.md        # How to extend safely
└── operations.md       # Config, health, troubleshooting
```

**Optional:** `sequence-diagram.md`, `troubleshooting.md` (use when flow is non-obvious).

---

## Naming conventions

| Rule | Example |
|------|---------|
| Folder names | lowercase kebab-case: `shared-kernel`, `building-blocks` |
| File names | lowercase kebab-case: `module-map.md` |
| ADR IDs | `ADR-NNNN-short-title.md` |
| Code identifiers in prose | Match codebase: `Ashraak.Auth.Api` |
| Product name | Ashraak (template); renamed per product in forks |

---

## Content rules

1. **Code-first** — Every claim traceable to a file path or config key.
2. **Honest status** — Label stubs: "Phase 2", "Scaffold", "Not wired".
3. **No Angular** — Frontend docs describe React 19 unless codebase changes.
4. **Version accuracy** — .NET 10 host, EF Core 9.0.4, React 19, Vite 6.
5. **Link, don't duplicate** — `DEVELOPER_GUIDE.md` remains the long-form rename/new-module tutorial; `/docs/extending` summarizes and links.
6. **Diagrams** — Mermaid in architecture docs where relationships help.

---

## Legacy document handling

| Legacy path | Role after standardization |
|-------------|---------------------------|
| `DEVELOPER_GUIDE.md` | Extended tutorial + rename prompt |
| `*_MODULE_STATUS.md` | Supplementary implementation snapshots |
| `DOCKER_ENVIRONMENT.md` | Deep Docker reference; `/docs/operations` summarizes |
| `FRONTEND_STARTER.md` | Starter reference; sync Bootstrap/CoreUI drift in follow-up |
| `API_COMPOSITION_ROOT.md` | Host detail; superseded by `/docs/modules/host/` |

**Do not delete** legacy files in this phase — reduces breakage for existing bookmarks.

---

## Governance enforcement (future phases)

| Trigger | Required documentation update |
|---------|------------------------------|
| New module | Full `docs/modules/{name}/` seven files + `module-map.md` + extension guide snippet |
| New endpoint | `api.md` for module + `/docs/api/overview.md` |
| New contract event | `events.md` for publisher and consumer modules |
| Middleware change | `docs/modules/host/operations.md` + architecture middleware section |
| Docker/env change | `getting-started/environment-variables.md` + `operations/deployment-notes.md` |
| Frontend route | `frontend/routing-and-guards.md` + feature module doc |
| Architectural decision | New ADR in `docs/adr/` |

**Merge blocker (recommended):** PR checklist item — "Documentation updated per governance.md".

---

## Migration completed in this phase

- [x] Audit reports (`documentation-audit/`)
- [x] Master index + governance
- [x] Architecture set (9 files)
- [x] ADR template + 5 initial ADRs
- [x] Getting started (6 files)
- [x] Module backfill (8 modules × 7 files)
- [x] Frontend docs (6 files)
- [x] API docs (4 files)
- [x] Extending guides (4 files)
- [x] Operations runbook (7 files)
- [x] Error docs (3 files)
- [x] Root README modernization

---

## Follow-up recommendations (code + doc sync)

Priority order for **future implementation phases** (not this doc phase):

1. Generate EF migrations or document manual schema bootstrap explicitly.
2. Wire `OutboxProcessorHostedService` or remove outbox claims from legacy docs.
3. Bind JWT signing key from configuration (match `.env.example`).
4. Sync `FRONTEND_STARTER.md` with CoreUI + sessionStorage reality.
5. Implement Audit GET or mark OpenAPI as deprecated until Phase 2.

---

## Success metrics

| Metric | Target |
|--------|--------|
| Every module has 7 standard files | 100% of listed modules |
| New developer can run stack from `/docs/getting-started` only | Yes |
| Architecture boundaries documented | Yes |
| Known stubs labeled | Yes |
| Governance file exists and is linked from README | Yes |
