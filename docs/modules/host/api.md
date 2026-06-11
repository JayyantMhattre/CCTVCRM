# Host — API Surface

The host exposes health endpoints, versioned REST (via modules), and unversioned OAuth protocol endpoints.

## Health

| Method | Path | Filter | Purpose |
|--------|------|--------|---------|
| GET | `/health` | All checks | Structured JSON aggregate |
| GET | `/health/live` | None (liveness) | Process alive |
| GET | `/health/ready` | `ready` tag | Postgres + Redis + MongoDB + Notifications + Outbox |

See [platform/health](../../platform/health/README.md).

## API versioning

Base prefix: `/api/v1/` (default version 1.0)

Alternative version readers:
- Query: `?api-version=1.0`
- Header: `x-api-version: 1.0`

## Versioned REST (via modules)

Full paths under `/api/v1/`:

| Module | Prefix | Endpoints doc |
|--------|--------|---------------|
| Auth | `/auth/` | [Auth API](../auth/api.md) |
| Tenant | `/tenants/` | [Tenant API](../tenant/api.md) |
| Users | `/users/` | [Users API](../users/api.md) |
| Audit | `/audit-logs/` | [Audit API](../audit/api.md) |

## Unversioned protocol (via Auth module)

Registered on root `WebApplication` — **not** under `/api/v1/`:

| Method | Path | Purpose |
|--------|------|---------|
| POST | `/connect/token` | Password grant token issuance |
| GET | `/api/auth/sso/google` | Google SSO challenge |
| GET | `/api/auth/sso/microsoft` | Microsoft SSO challenge |
| GET | `/api/auth/sso/callback` | SSO completion (returns external claims JSON) |

OpenIddict also configures (no explicit minimal API handlers):
- `/connect/authorize`
- `/connect/introspect`
- `/connect/logout`

## Development OpenAPI

| Path | Purpose |
|------|---------|
| `/openapi/v1.json` | OpenAPI document |
| `/scalar/v1` | Scalar UI |

Only mapped when `IsDevelopment()`.

## Authorization policies (endpoint metadata)

| Policy | Used by |
|--------|---------|
| `AllowAnonymous` | Register, token, SSO, provision tenant |
| `RequireAuthorization()` | Most tenant/user endpoints |
| `AdminOnly` | `GET /api/v1/audit-logs` |

## Output cache

Global 30-second base policy via `UseOutputCache()`. Redis-backed. Applied after audit middleware.

## Error responses

`GlobalExceptionHandler` (`Middleware/GlobalExceptionHandler.cs`):
- Unhandled exceptions → 500 ProblemDetails (RFC 7807)
- Module handlers return `Results.Problem()` for business failures (400, 404, etc.)

## Frontend expected paths

React app (`FrontEnd/apps/web/`):

| Client call | Backend route |
|-------------|---------------|
| `POST /connect/token` | `/connect/token` ✓ |
| `POST /api/v1/auth/register` | `/api/v1/auth/register` ✓ |
| SSO (may vary) | `/api/auth/sso/*` (unversioned) |

Dev proxy in `vite.config.ts` forwards `/api` and `/connect` to the API base URL.

## Infrastructure types (not HTTP)

| Type | File | Role |
|------|------|------|
| `CurrentUser` | `Infrastructure/CurrentUser.cs` | Reads `sub`, `email`, roles from claims |
| `TenantContext` | `Infrastructure/TenantContext.cs` | Reads `tenant_id`/`tenantId` claim or header |
| `DateTimeProvider` | `Infrastructure/DateTimeProvider.cs` | `UtcNow` |
| `GlobalExceptionHandler` | `Middleware/GlobalExceptionHandler.cs` | Central exception mapping |
