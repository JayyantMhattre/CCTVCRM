# Audit viewer

Admin audit log UI at `/audit`.

**Page:** `FrontEnd/apps/web/src/modules/audit/pages/AuditLogPage.tsx`

**API:** `auditApi.getLogs()` → `GET /api/v1/audit-logs`

## Features

- Module, event type, search, date range filters
- Server-side pagination (`page`, `pageSize`, `totalCount`)
- Loading, empty, and error states with correlation ID
- Correlation ID on load failures

## Access

- Route: `RoleGuard` with `Admin` role
- Sidebar: `PermissionGuard` with `audit:read`

## Related

- [routing.md](./routing.md)
- [filters.md](./filters.md)
- [API audit](../../api/audit.md)
