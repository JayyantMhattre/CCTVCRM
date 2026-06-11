# API Keys platform manifest

Last updated: V1 API Keys implementation

---

## Capabilities

| Capability | Status | Notes |
|------------|--------|-------|
| API key generation | Done | `ashk_{env}_*` format |
| Argon2id hashing | Done | ADR-ApiKeys-0001 |
| M2M authentication | Done | X-API-Key + Bearer |
| Scopes | Done | `resource:action` |
| Tenant isolation | Done | EF query filter |
| Rotation | Done | Manual, audit logged |
| Revocation | Done | Immediate |
| Usage tracking | Done | Counters + correlation |
| Rate limiting | Done | Per API key subject |
| Audit events | Done | Domain event auto-audit |
| Admin UI (web) | Done | `/api-keys` |
| Mobile visibility | Done | Read-only |
| Health checks | Done | `/health/ready` |

---

## Client coverage

| Client | Create | Rotate | Revoke | Read usage |
|--------|--------|--------|--------|------------|
| Backend | Done | Done | Done | Done |
| Web | Done | Done | Done | Done |
| Mobile | N/A | N/A | N/A | Done (read) |
