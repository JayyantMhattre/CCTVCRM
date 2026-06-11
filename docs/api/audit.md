# Audit API

Implementation: `BackEnd/src/Modules/Audit/Ashraak.Audit.Api/Endpoints/AuditEndpoints.cs`

---

## GET /api/v1/audit-logs

Query audit entries.

**Auth:** Policy `AdminOnly` (role `Admin`)

**Query parameters:**

| Param | Type | Description |
|-------|------|-------------|
| `tenantId` | guid? | Filter by tenant |
| `module` | string? | Source module name |
| `from` | datetime? | UTC start |
| `to` | datetime? | UTC end |
| `page` | int | Default 1 |
| `pageSize` | int | Default 50, max 100 |

**Current response (stub):**

```json
{
  "message": "Audit log query — implement with Dapper read model in Phase 2",
  "filters": { "tenantId": null, "module": null, "from": null, "to": null, "page": 1, "pageSize": 50 }
}
```

**Phase 2:** MongoDB aggregation + hash chain validation endpoint.

---

## Capture (not HTTP)

Audit data is written via:

- `AuditApiCallMiddleware` — all versioned API calls
- `AuditEntityChangeInterceptor` — EF SaveChanges
- `DomainEventAuditHandler` — all `IDomainEvent`

See [modules/audit/architecture.md](../modules/audit/architecture.md).

---

## Related

- [modules/audit/api.md](../modules/audit/api.md)
