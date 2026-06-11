# Required configuration

## Connection strings (all environments)

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL default |
| `ConnectionStrings:Auth` | Auth schema |
| `ConnectionStrings:Tenant` | Tenant schema |
| `ConnectionStrings:Users` | Users schema |
| `ConnectionStrings:Redis` | Cache + rate limits |
| `ConnectionStrings:MongoDB` | Audit store |

## Application sections

| Section | Required keys |
|---------|---------------|
| `Outbox` | `PollInterval`, `BatchSize` (> 0) |
| `Notifications` | `Provider`, `TemplatesPath` |

## Production-only

| Key | Purpose |
|-----|---------|
| `Seq:Url` | Centralised logging |
| `Authentication:Jwt:SigningKey` or `OpenIddict:SigningKey` | Stable token signing (not ephemeral dev keys) |

Inject via environment variables in Docker/Kubernetes — see [getting-started/environment-variables.md](../../getting-started/environment-variables.md).
