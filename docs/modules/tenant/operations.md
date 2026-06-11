# Tenant — Operations

## Prerequisites

| Dependency | Config key |
|------------|------------|
| PostgreSQL (tenant schema) | `ConnectionStrings:Tenant` |
| Redis | `ConnectionStrings:Redis` (TenantService cache) |

## Configuration

```
ConnectionStrings:Tenant=Host=localhost;Port=5432;Database=ashraak;...;Search Path=tenant
```

Fallback: `ConnectionStrings:DefaultConnection`

## Provision a tenant

```http
POST /api/v1/tenants
Content-Type: application/json

{
  "name": "Demo Org",
  "slug": "demo-org",
  "plan": "Free",
  "ownerUserId": null
}
```

No auth required. Save returned tenant ID for user registration.

## Query tenant

```http
GET /api/v1/tenants/current
Authorization: Bearer {token}
X-Tenant-ID: {tenantId}
```

## Cache behavior

| Data | TTL | Invalidation |
|------|-----|--------------|
| Tenant config | 5 minutes | TTL expiry only |
| Feature flags | 5 minutes | TTL expiry only |
| Seat limit | Uncached | N/A |

Flush Redis keys matching `{env}:{tenantId}:tenant:*` to force refresh.

## Database

Schema: `tenant`

Tables: `tenants` (with owned settings/subscription columns), `outbox_messages` (scaffold).

Unique constraint: `slug`.

## Troubleshooting

| Issue | Cause | Action |
|-------|-------|--------|
| 403 on GET tenant | Cross-tenant access | Use matching token tenant |
| 403 on login | Tenant suspended/deleted | Check `TenantStatus` in DB |
| Stale plan in Auth | Redis cache | Wait 5 min or flush cache |
| Slug conflict on provision | Duplicate slug | Choose unique slug |
| Users not deactivated on delete | `TenantDeletedEvent` not published | Publish contract event |

## Health

No module-specific check. Postgres covered by host `/health/ready`.

## Related

- [Auth operations](../auth/operations.md) — requires active tenant
- [Caching operations](../caching/operations.md) — Redis keys
