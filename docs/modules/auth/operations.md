# Auth — Operations

## Prerequisites

| Dependency | Config key |
|------------|------------|
| PostgreSQL (auth schema) | `ConnectionStrings:Auth` |
| Redis | `ConnectionStrings:Redis` (permissions + sessions via Caching) |
| Tenant module | Active tenant for register/login |

## Configuration

| Key | Example | Notes |
|-----|---------|-------|
| `ConnectionStrings:Auth` | `Host=localhost;...;Search Path=auth` | Schema isolation |
| `Authentication:Google:ClientId` | OAuth client ID | SSO |
| `Authentication:Google:ClientSecret` | Secret | SSO |
| `Authentication:Microsoft:ClientId` | OAuth client ID | SSO |
| `Authentication:Microsoft:ClientSecret` | Secret | SSO |
| `Auth:SigningKeyBase64` | Base64 key | **Not used** — ephemeral keys active |

Docker: `Authentication__Google__*`, `Authentication__Microsoft__*`, `JWT_SIGNING_KEY_BASE64` (unused).

## Token operations

### Obtain token

```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&username=user@example.com&password=Secret123&tenant_id={guid}
```

Or pass `X-Tenant-ID` header instead of `tenant_id` form field.

### Use token

```http
GET /api/v1/users/{userId}
Authorization: Bearer {access_token}
```

Validation via OpenIddict validation middleware (`UseLocalServer`).

### After app restart

All tokens become **invalid** due to ephemeral signing keys. Users must re-authenticate.

## SSO operations

1. Navigate to `/api/auth/sso/google` or `/api/auth/sso/microsoft`
2. Complete OAuth at provider
3. Callback returns external claims at `/api/auth/sso/callback`
4. No local session created until account linking is implemented

Ensure OAuth redirect URIs match registered callbacks:
- `/api/auth/sso/google/callback`
- `/api/auth/sso/microsoft/callback`

## Cache TTLs

| Data | Key pattern | TTL |
|------|-------------|-----|
| Permissions | `{env}:{tenant}:auth:perms:{userId}` | 10 minutes |
| Session | `{env}:{tenant}:session:{userId}` | 8 hours |

Invalidate manually via Redis CLI if permissions change and cache stale, or wait for TTL.

## Database

Schema: `auth`

Tables include Identity, OpenIddict, RBAC/ABAC authorization, `outbox_messages` (scaffold).

No EF migrations in repo at time of writing — apply schema manually or generate migrations.

## Troubleshooting

| Issue | Cause | Fix |
|-------|-------|-----|
| `invalid_grant` on token | Wrong password, inactive tenant, locked user | Check tenant status, credentials |
| 401 after restart | Ephemeral keys rotated | Re-login |
| 403 on API | Inactive tenant | Activate tenant in DB |
| 400 tenant mismatch | JWT tenant ≠ header | Align `X-Tenant-ID` with token |
| SSO redirect error | Wrong callback URL at provider | Match registered URIs |
| Permissions stale | Redis cache | Wait 10 min or flush key |
| Register 400 duplicate | Email exists in tenant | Use different email |
| Users profile missing | `UserRegisteredEvent` handler failed | Check Users module logs |

## Health

Auth has no dedicated health check. Postgres readiness via host `/health/ready`.

## Security operations

- Rotate OAuth secrets at Google/Microsoft consoles
- Plan migration to persistent `Auth:SigningKeyBase64` before production
- Monitor failed login attempts via audit logs (MongoDB) and Serilog

## Related docs

- [Host operations](../host/operations.md) — pipeline, Docker
- [Caching operations](../caching/operations.md) — Redis connectivity
- [Users events](../users/events.md) — `UserRegisteredEvent` consumer
