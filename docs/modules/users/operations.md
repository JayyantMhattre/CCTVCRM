# Users — Operations

## Prerequisites

| Dependency | Config key |
|------------|------------|
| PostgreSQL (users schema) | `ConnectionStrings:Users` |
| Auth module | Profile created via `UserRegisteredEvent` |
| Tenant context | JWT or `X-Tenant-ID` for filtered queries |

## Configuration

```
ConnectionStrings:Users=Host=localhost;Port=5432;Database=ashraak;...;Search Path=users
```

## Typical flow after registration

1. `POST /api/v1/auth/register` → returns `userId`
2. `UserRegisteredEventHandler` creates profile (same request, synchronous)
3. `GET /api/v1/users/{userId}` with token → returns profile

If step 3 returns 404, check handler logs — event may have failed silently.

## Query users

```http
GET /api/v1/users/tenant/current
Authorization: Bearer {token}
```

Tenant resolved from token claims via middleware.

## Database

Schema: `users`

| Table | Purpose |
|-------|---------|
| `profiles` | User profile + preferences columns |
| `tenant_memberships` | Role per tenant |
| `outbox_messages` | Scaffold |

Unique: `(email, tenant_id)`.

Global filter: all queries scoped to `ITenantContext.TenantId`.

## Troubleshooting

| Issue | Cause | Action |
|-------|-------|--------|
| 404 after register | Event handler failed | Check logs; verify users schema |
| Empty tenant list | Wrong tenant context | Verify JWT `tenant_id` claim |
| 403 on tenant users | Cross-tenant request | Match token tenant to URL |
| Profiles not deactivated on tenant delete | `TenantDeletedEvent` not published | Fix Tenant module event publish |
| Duplicate email | Unique index | Email unique per tenant |

## Health

Postgres via host `/health/ready`. No Users-specific check.

## Related

- [Auth operations](../auth/operations.md) — registration
- [Tenant events](../tenant/events.md) — `TenantDeletedEvent` gap
