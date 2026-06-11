# Audit — Operations

## Prerequisites

| Dependency | Config key |
|------------|------------|
| MongoDB | `ConnectionStrings:MongoDB` (**required**) |

Startup throws if MongoDB connection string is missing.

## Configuration

```
ConnectionStrings:MongoDB=mongodb://localhost:27017/ashraak_audit
```

Docker: `ConnectionStrings__MongoDB` env var.

Database name: from URI path or default `ashraak_audit`.  
Collection: `audit_entries`.

## Verify MongoDB connectivity

```powershell
curl http://localhost:5000/health/ready
```

MongoDB included in ready check tags.

Direct Mongo:

```javascript
use ashraak_audit
db.audit_entries.find().sort({ OccurredOnUtc: -1 }).limit(5)
```

## Verify writes occurring

1. Authenticated API call with valid tenant
2. Check Mongo for `Module: "API"`, `Action: "ApiCall"`
3. EF change → `Action: "Created"` / `"Modified"` / `"Deleted"`
4. Register user → domain event entry if `UserRegisteredEvent` published

Background writer runs in `AuditMongoWriterHostedService` — check logs for write errors.

## Query stub endpoint

```http
GET /api/v1/audit-logs?page=1&pageSize=50
Authorization: Bearer {admin-token}
```

Requires `Admin` role. Returns placeholder — use MongoDB directly for data until Phase 2.

## Hash chain

Each insert computes SHA-256 over payload + `PreviousHash` for tenant. Tamper detection planned via integrity endpoint.

If writer fails mid-chain, investigate `AuditMongoWriterHostedService` logs.

## Index maintenance

Indexes created at app startup. If collection pre-exists with different indexes, review `AuditModule.EnsureIndexes`.

## Troubleshooting

| Issue | Cause | Action |
|-------|-------|--------|
| Startup failure | Missing MongoDB config | Set `ConnectionStrings:MongoDB` |
| No audit entries | TenantId empty in middleware | Ensure authenticated + tenant resolved |
| Missing entity audits | Interceptor not registered | Verify Audit module registered (Layer 3) |
| Missing event audits | Event not published via MediatR | Check handler publishes after save |
| Queue backup | Writer slower than producers | Scale writer, batch inserts |
| 403 on audit-logs | Not Admin role | Use Admin JWT |
| OutboxMessage audited | Should be excluded | Verify interceptor skip logic |

## Retention

No TTL index by default — plan disk usage for `audit_entries`.

## Related

- [Host operations](../host/operations.md) — middleware order
- [MongoDB docker](../host/operations.md) — compose services

## Logs

Serilog captures hosted service exceptions. OpenTelemetry traces HTTP requests but audit writes are background.
