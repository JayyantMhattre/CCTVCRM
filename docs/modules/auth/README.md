# Auth Module

Identity, authentication, and authorization for Ashraak. Uses OpenIddict with ephemeral signing keys, ASP.NET Identity for user storage, Argon2 password hashing, and RBAC/ABAC permission checks cached in Redis.

**Source:** `BackEnd/src/Modules/Auth/`

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Auth.Domain` |
| Application | `Ashraak.Auth.Application` |
| Infrastructure | `Ashraak.Auth.Infrastructure` |
| Api | `Ashraak.Auth.Api` |

## Key facts

- **OpenIddict 5.4.0** with **ephemeral encryption and signing keys** — tokens invalid after restart
- **Token endpoint:** `POST /connect/token` (unversioned, custom password-grant handler)
- **REST:** `POST /api/v1/auth/register`
- **SSO:** Google and Microsoft at `/api/auth/sso/*` (unversioned)
- **Tenant resolution middleware** runs in host pipeline after authentication
- **`Auth__SigningKeyBase64`** in Docker is **not consumed** by code today

## Module documentation

- [Architecture](./architecture.md) — domain model, OpenIddict, security
- [Registration](./registration.md) — DI and OpenIddict setup
- [API](./api.md) — all endpoints and request/response shapes
- [Events](./events.md) — published and consumed events
- [Extending](./extending.md) — new flows, persistent keys, roles
- [Operations](./operations.md) — config, SSO, troubleshooting

## Dependencies

- [Caching](../caching/README.md) — `ICacheService`, `ISessionCacheService`
- [Tenant](../tenant/README.md) — `ITenantService` for active check
- [Shared Kernel](../shared-kernel/README.md) — `IAuthPermissionChecker` contract
