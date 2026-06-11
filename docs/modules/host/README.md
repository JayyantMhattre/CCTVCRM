# Host (Ashraak.Api)

The API composition root for the Ashraak modular monolith. Single `Program.cs` bootstraps Serilog, OpenTelemetry, health checks, API versioning, module plug-in registration, and the HTTP pipeline.

**Source:** `BackEnd/src/Host/Ashraak.Api/`

**Target:** .NET 10, EF Core 9.0.4 (via module DbContexts)

## Responsibilities

- Register cross-cutting services (`ICurrentUser`, `ITenantContext`, `IDateTimeProvider`)
- Orchestrate module DI via `AddModules()` — order: Caching → Auth → Tenant → Users → Audit
- Map versioned REST at `/api/v{version:apiVersion}` and unversioned OAuth at root
- Configure authorization policies (`AdminOnly`, `TenantAdmin`)
- Output cache (Redis-backed, 30s base policy)
- Health checks: `/health`, `/health/live`, `/health/ready` (postgres, redis, mongodb, notifications, outbox)
- Correlation middleware, Redis rate limiting, startup env validation, feature-flag foundation
- OpenAPI + Scalar in Development

## What the host does not do

- Serve the React SPA (separate Vite app at `FrontEnd/apps/web/`)
- Configure CORS for frontend (dev proxy handles `/api` and `/connect`)
- Register RabbitMQ or MassTransit
- Run outbox processor or Quartz jobs

## Module documentation

- [Architecture](./architecture.md) — pipeline, versioning, project layout
- [Registration](./registration.md) — DI, module plug-in pattern
- [API](./api.md) — health, versioning, endpoint map
- [Events](./events.md) — host role in event flow
- [Extending](./extending.md) — adding modules, middleware, policies
- [Operations](./operations.md) — config, Docker, observability

## Related modules

- [Auth](../auth/README.md) — OpenIddict, tenant resolution middleware
- [Caching](../caching/README.md) — registered first (Layer 0)
- [Audit](../audit/README.md) — API call logging middleware

## Frontend (React)

Dev SPA: `FrontEnd/apps/web/` — Vite port 3000, proxies `/api` and `/connect` to API (default `http://localhost:5000`). See [architecture.md](./architecture.md#frontend-integration).
