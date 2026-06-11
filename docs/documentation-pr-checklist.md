# Pull Request Documentation Checklist

Copy into PR descriptions when changing behavior. Required per [documentation-governance.md](./documentation-governance.md).

---

## Code ↔ documentation

- [ ] Audited implementation — docs match **code**, not assumptions
- [ ] Updated canonical docs under `/docs/` (not only legacy `*_MODULE_STATUS.md`)
- [ ] CI doc validation considered (`scripts/validate-docs.ps1` or GitHub Action)

---

## Scope-specific updates

| If you changed… | Update… |
|----------------|---------|
| Module `BackEnd/src/Modules/{Name}/` | `docs/modules/{name}/` (all impacted files) |
| Host `Program.cs`, `ModuleExtensions.cs` | `docs/modules/host/`, `docs/architecture/` |
| SharedKernel / Contracts | `docs/modules/shared-kernel/` |
| BuildingBlocks | `docs/modules/building-blocks/` |
| HTTP endpoints | Module `api.md` + `docs/api/` |
| Domain / contract events | Module `events.md` + consumer modules |
| Frontend `src/modules/` or `src/core/` | `docs/frontend/` |
| Docker / env | `docs/getting-started/environment-variables.md`, `BackEnd/DOCKER_ENVIRONMENT.md` |
| New module | Full 7-file module doc set + `docs/architecture/module-map.md` |

---

## README and onboarding

- [ ] Root `README.md` updated if setup ports, commands, or stack changed
- [ ] `docs/getting-started/*` updated if local workflow changed
- [ ] `DEVELOPER_GUIDE.md` updated only for long-form tutorial deltas (link to `/docs` for details)

---

## Architecture and decisions

- [ ] **ADR required?** — New boundary, datastore, auth model, or irreversible pattern → `docs/adr/ADR-NNNN-*.md`
- [ ] `docs/architecture/module-map.md` updated for new/removed modules
- [ ] Extension guides updated if template patterns changed

---

## API and errors

- [ ] OpenAPI metadata in endpoint code (`WithSummary`, `WithDescription`) when adding routes
- [ ] `docs/api/` updated for new/changed endpoints
- [ ] `docs/errors/error-catalog.md` for new error codes

---

## Operations

- [ ] `docs/operations/` if logging, health, Redis, Seq, or deployment behavior changed
- [ ] Troubleshooting section if new known failure mode

---

## Stubs and limitations

- [ ] Labeled **Implemented** vs **Stub** vs **Scaffold** (no aspirational docs)
- [ ] Outbox / RabbitMQ / JWT signing docs honest if touching those areas

---

## Legacy doc sync (when touching legacy files)

- [ ] `FRONTEND_STARTER.md` aligned with `docs/frontend/` if frontend behavior changed
- [ ] `*_MODULE_STATUS.md` does not contradict `docs/modules/*/`

---

## Reviewer gate

**Approve only when** checklist is complete or exceptions are explained in PR comments.

Automated hint: GitHub workflow **Documentation Validation** (warn by default).
