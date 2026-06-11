# API Overview

## Base URLs

| Environment | Base URL |
|-------------|----------|
| Local Kestrel | `http://localhost:5000` |
| Docker API | `http://localhost:8080` |
| Frontend proxy | `http://localhost:3000` (proxies `/api`, `/connect`) |

## Versioning

REST endpoints use URL segment versioning:

```
/api/v1/{module}/...
```

Version readers also accept:

- Query: `?api-version=1.0`
- Header: `x-api-version: 1.0`

**Unversioned** (stable forever):

- `POST /connect/token`
- `GET /api/auth/sso/*`

## Authentication

| Flow | Endpoint | Grant |
|------|----------|-------|
| Login | `POST /connect/token` | `password` (resource owner) |
| API calls | Any versioned route | `Authorization: Bearer {access_token}` |

Required form fields for token (see [auth.md](./auth.md)):

- `grant_type=password`
- `username` (email)
- `password`
- `tenant_id` (or header `X-Tenant-ID`)

## Authorization

JWT claims:

- `role` — RBAC (e.g. `Admin`, `Manager`)
- `permission` — ABAC strings (e.g. `audit:read`)
- `tenant_id` / `tenantId` — multi-tenancy

Policies (host):

- `AdminOnly` — Audit endpoints
- `TenantAdmin` — declared, **unused on endpoints today**

## Response formats

| Type | Format |
|------|--------|
| Success | JSON body |
| Validation / app errors | RFC 7807 ProblemDetails |
| OAuth errors | `{ error, error_description }` JSON |

## Interactive documentation

Development only:

- Scalar UI: `/scalar/v1`
- OpenAPI JSON: `/openapi/v1.json`

## Health (unversioned)

| Endpoint | Purpose |
|----------|---------|
| `GET /health/live` | Liveness |
| `GET /health/ready` | Readiness (postgres, redis, mongodb) |

## Module API index

| Module | Doc |
|--------|-----|
| Auth | [auth.md](./auth.md) |
| Tenant | [tenant.md](./tenant.md) |
| Users | [users.md](./users.md) |
| Audit | [audit.md](./audit.md) |

## Stub endpoints

| Endpoint | Status |
|----------|--------|
| `GET /api/v1/audit-logs` | Returns placeholder JSON — Phase 2 |
