# Audit — API

Base path: `/api/v1/audit-logs/` (host version group + `MapAuditEndpoints`)

File: `BackEnd/src/Modules/Audit/Ashraak.Audit.Api/Endpoints/AuditEndpoints.cs`

## GET /api/v1/audit-logs

| Property | Value |
|----------|-------|
| Auth | `AdminOnly` policy (role `"Admin"`) |
| Implementation | `IAuditReadService` — MongoDB `audit_entries` |

**Query parameters:**

| Parameter | Default | Notes |
|-----------|---------|-------|
| `module` | — | Filter by module name |
| `search` | — | Text search |
| `from` | — | Date range start (UTC) |
| `to` | — | Date range end (UTC) |
| `page` | 1 | Page number |
| `pageSize` | 50 | Page size |

**Response:** `AuditLogPageDto` with `items`, `page`, `pageSize`, `totalCount`.

---

## Write API (internal contract, not HTTP)

**`IAuditService.LogAsync(AuditEntryDto)`**

Path: `SharedKernel.Contracts/Audit/Interfaces/IAuditService.cs`

Called by:
- `AuditApiCallMiddleware`
- `AuditEntityChangeInterceptor`
- `DomainEventAuditHandler`
- Any module via DI (singleton)

**Not exposed as public HTTP endpoint** — write-only through internal services.

---

## AuditEntryDto fields

Path: `SharedKernel.Contracts/Audit/Dtos/AuditEntryDto.cs`

Used for enqueue before mapping to `AuditEntry` entity:

- `TenantId`, `UserId`, `Module`, `Action`, `EntityType`, `EntityId`
- `OldValues`, `NewValues`, `IpAddress`, `UserAgent`, `OccurredOnUtc`

---

## Middleware skip paths

These paths are **not** logged by `AuditApiCallMiddleware`:

- `/health`
- `/connect`
- `/api/audit-logs`

Domain events and EF changes on other paths are still captured.

---

## Future endpoints (Phase 2)

Planned in `AUDIT_MODULE_STATUS.md`:

- Real paginated query against MongoDB or Dapper read model
- Hash-chain integrity validation endpoint
