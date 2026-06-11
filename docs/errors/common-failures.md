# Common Failures

Quick diagnosis table for local development.

---

## Infrastructure

| Failure | Cause | Resolution |
|---------|-------|------------|
| `Connection refused` postgres | Docker not running | `docker compose up -d postgres` |
| Redis timeout | Wrong host from host-machine vs container | Use `localhost:6379` for local `dotnet run` |
| Mongo auth failed | Credentials mismatch | Match `.env` with compose |
| Seq unreachable | Container stopped | Optional for dev — remove Seq sink temporarily |

---

## Authentication

| Failure | Cause | Resolution |
|---------|-------|------------|
| `invalid_grant` | Wrong password/email | Verify user in `auth.users` |
| `invalid_tenant` | Missing tenant_id in form | Include in login body |
| `tenant_inactive` | Tenant suspended | Check tenant status |
| Token works then all 401 | API restarted | Re-login — ephemeral signing keys |
| 403 on tenant GET | JWT tenant ≠ requested id | Use `/tenants/current` |

---

## Data

| Failure | Cause | Resolution |
|---------|-------|------------|
| relation does not exist | No migrations | Add EF migrations or init SQL |
| User profile 404 after register | Users handler failed | Check logs for `UserRegisteredEventHandler` |
| Empty audit query | Stub endpoint | Expected — see Phase 2 |

---

## Frontend

| Failure | Cause | Resolution |
|---------|-------|------------|
| CORS error | Calling API without proxy | Use `pnpm dev` not static file |
| Redirect loop login | Refresh token invalid | Clear sessionStorage |
| 403 on /users | User lacks Admin/Manager role | Assign role in auth data |
| Audit page empty | Backend stub | Expected |

---

## Cross-module

| Failure | Cause | Resolution |
|---------|-------|------------|
| Event not handled | Handler not registered | Check `AddMediatR` assembly |
| Outbox event lost | Outbox not implemented | Use sync publish pattern today |

---

## Related

- [startup-troubleshooting.md](../operations/startup-troubleshooting.md)
- [documentation-audit/outdated-docs-report.md](../documentation-audit/outdated-docs-report.md)
