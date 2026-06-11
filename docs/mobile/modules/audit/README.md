# Mobile — Audit viewer

**Feature:** `FrontEnd.Mobile/lib/features/audit/`

Admin-only read model — no analytics.

## API

`GET /api/v1/audit-logs` — requires **Admin** role.

### Query parameters

| Param | Purpose |
|-------|---------|
| `module` | Module name filter |
| `search` | Regex on action/module/entityType |
| `from` / `to` | UTC date range |
| `page` / `pageSize` | Paging (default 25) |

`eventType` filter applied **client-side** on current page (backend does not accept this param yet).

## Display

| Column | Field |
|--------|-------|
| Timestamp | `occurredOnUtc` |
| Event | `action` |
| Module | `module` |
| User | `userId` |
| Reference | `id` (audit entry ID; copyable) |

## Provider

`AuditProvider` — filters, paging, load.

## Route

`/audit` — redirect to `/home` if user lacks Admin role.
