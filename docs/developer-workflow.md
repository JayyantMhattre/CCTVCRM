# Developer Workflow

Simple loop for keeping Ashraak documentation accurate and enforced.

---

## Standard workflow

```
1. Implement change (code)
2. Update documentation (same PR)
3. Add ADR if architectural decision
4. Run local doc validation
5. Complete PR checklist → open PR
```

---

## 1. Code

- Follow existing module patterns — see [extending/add-backend-module.md](./extending/add-backend-module.md)
- Do not add cross-module Infrastructure references
- Prefer contracts and events in `SharedKernel.Contracts`

---

## 2. Documentation (mandatory)

**Canonical location:** `/docs/`

| Change type | Minimum docs |
|-------------|--------------|
| Backend module | `docs/modules/{name}/` — update every affected file |
| API endpoint | Module `api.md` + `docs/api/` |
| Frontend feature | `docs/frontend/` |
| Config / Docker | `environment-variables.md`, `DOCKER_ENVIRONMENT.md` |

**Governance:** [documentation-governance.md](./documentation-governance.md)

**PR checklist:** [documentation-pr-checklist.md](./documentation-pr-checklist.md)

Legacy files (`FRONTEND_STARTER.md`, `*_MODULE_STATUS.md`) must not contradict `/docs/`.

---

## 3. ADR (when required)

Create `docs/adr/ADR-NNNN-short-title.md` when you:

- Add or remove a module
- Change integration style (sync events → outbox, new datastore)
- Adopt a new cross-cutting pattern

Use [ADR-0000-template.md](./adr/ADR-0000-template.md).

---

## 4. Local validation

### Documentation diff check

**PowerShell (Windows):**

```powershell
.\scripts\validate-docs.ps1 -BaseRef origin/main
# Hard gate locally:
.\scripts\validate-docs.ps1 -BaseRef origin/main -Mode fail
```

**Bash (Linux / macOS / CI):**

```bash
./scripts/validate-docs.sh origin/main
DOC_VALIDATE_MODE=fail ./scripts/validate-docs.sh origin/main
```

**Configuration:** [doc-validation.json](./doc-validation.json)

| Mode | Behavior |
|------|----------|
| `warn` (default) | Prints violations; exit 0 |
| `fail` | Exit 1 if code changed without mapped doc updates |

Set repository variable `DOC_VALIDATE_MODE=fail` in GitHub for enforced CI.

---

## 5. Pull request

1. Paste checklist from [documentation-pr-checklist.md](./documentation-pr-checklist.md)
2. Ensure **Documentation Validation** workflow passes or explain exceptions
3. Link ADR in PR description if added

---

## What triggers doc validation

| Code path | Expected doc paths |
|-----------|-------------------|
| `BackEnd/src/Modules/Auth/` | `docs/modules/auth/`, `docs/api/auth.md` |
| `BackEnd/src/Modules/Tenant/` | `docs/modules/tenant/`, … |
| `BackEnd/src/Modules/Users/` | `docs/modules/users/`, … |
| `BackEnd/src/Modules/Audit/` | `docs/modules/audit/`, … |
| `BackEnd/src/Modules/Caching/` | `docs/modules/caching/` |
| `BackEnd/src/Shared/` | `docs/modules/shared-kernel/` |
| `BackEnd/src/BuildingBlocks/` | `docs/modules/building-blocks/` |
| `BackEnd/src/Host/` | `docs/modules/host/`, `docs/architecture/`, `docs/operations/` |
| `FrontEnd/apps/web/src/core/` | `docs/frontend/` |
| `FrontEnd/apps/web/src/modules/` | `docs/frontend/`, `docs/api/` |

Docs-only PRs skip validation.

---

## AI-assisted development

When using Cursor or other agents:

1. Start from [docs/index.md](./index.md)
2. Require agent to update `/docs/` in the same task as code
3. Run `validate-docs` before commit

---

## Related

- [documentation-governance.md](./documentation-governance.md)
- [documentation-audit/outdated-docs-report.md](./documentation-audit/outdated-docs-report.md)
