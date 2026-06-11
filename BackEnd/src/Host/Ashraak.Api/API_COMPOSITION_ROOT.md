# API Composition Root — `Ashraak.Api`

> **Canonical:** [docs/modules/host/](../../../docs/modules/host/README.md)

## Overview

`Ashraak.Api` is the **single host entry point** for the modular monolith.  
Its only responsibilities are:

1. Bootstrap the .NET host and configure logging.
2. Register all feature modules into the DI container.
3. Define the HTTP middleware pipeline.
4. Expose a versioned route group and hand it to each module for endpoint registration.

The host project **never** imports domain or application logic from any module.  
All cross-module wiring is done through `SharedKernel.Contracts` interfaces and domain events.

---

## Project Structure

```
src/Host/Ashraak.Api/
├── Program.cs                          ← Composition root (entry point)
├── Ashraak.Api.csproj                  ← References all module Infrastructure + Api projects
│
├── Extensions/
│   ├── ModuleExtensions.cs             ← AddModules() / MapModules() / MapModuleProtocolEndpoints()
│   └── OpenApiExtensions.cs            ← AddOpenApiDocs() / MapOpenApiDocs()
│
├── Infrastructure/
│   ├── CurrentUser.cs                  ← ICurrentUser implementation (reads JWT claims)
│   ├── TenantContext.cs                ← ITenantContext implementation (reads JWT/header)
│   └── DateTimeProvider.cs             ← IDateTimeProvider (UTC clock abstraction)
│
└── Middleware/
    └── GlobalExceptionHandler.cs       ← IExceptionHandler → RFC 7807 ProblemDetails
```

---

## Module Registration Pattern

All feature modules expose two static extension method contracts:

```csharp
// DI registration
services.Add{Module}(IServiceCollection, IConfiguration)

// Endpoint registration
routeBuilder.Map{Module}Endpoints(IEndpointRouteBuilder)
```

`ModuleExtensions.cs` orchestrates both in a single call from `Program.cs`:

```csharp
// ── DI ──
builder.Services.AddModules(builder.Configuration);

// ── Endpoints ──
v1.MapModules();                       // versioned REST endpoints
app.MapModuleProtocolEndpoints();      // unversioned protocol endpoints
```

### Adding a new module

1. Create `src/Modules/{Name}/` with `Domain / Application / Infrastructure / Api` layers.
2. Add `Add{Name}Module()` extension in `{Name}.Infrastructure/{Name}Module.cs`.
3. Add `Map{Name}Endpoints()` extension in `{Name}.Api/Endpoints/{Name}Endpoints.cs` — use **no** `/api/` prefix (the host provides it).
4. Add one line to `ModuleExtensions.cs`:  
   `AddModules` → `services.Add{Name}Module(configuration);`  
   `MapModules` → `routeBuilder.Map{Name}Endpoints();`

No other host file needs to change.

---

## API Versioning

### Strategy

| Reader | Example |
|--------|---------|
| **URL segment** (primary) | `GET /api/v1/tenants/{id}` |
| Query string (convenience) | `GET /api/tenants/{id}?api-version=1` |
| Header (testing) | `x-api-version: 1` |

`AssumeDefaultVersionWhenUnspecified = true` ensures health checks and
protocol endpoints at root paths continue to work without a version segment.

### Current versions

| Version | Status | Notes |
|---------|--------|-------|
| `v1` | Stable | All current modules |

### Adding a new version

```csharp
// 1. Extend the ApiVersionSet in Program.cs
var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))   // ← add
    .Build();

// 2. Create a v2 route group
var v2 = app.MapGroup("/api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet)
    .MapToApiVersion(new ApiVersion(2, 0));

// 3. Register only the module endpoints that have a v2 variant
v2.MapTenantEndpoints();   // assuming Tenant module added v2 support
```

---

## Middleware Pipeline

```
HTTP Request
     │
     ▼
[1] UseExceptionHandler          — RFC 7807 ProblemDetails on any unhandled exception
     │
[2] UseSerilogRequestLogging     — Structured access log (method, path, status, duration)
     │
[3] UseAuthentication            — Validates JWT via OpenIddict; populates ClaimsPrincipal
     │
[4] UseTenantResolution          — Resolves TenantId from JWT / X-Tenant-ID header
     │                             Rejects requests to inactive tenants (403)
     │                             Blocks mismatched tenant IDs (403)
     │
[5] UseAuthorization             — Evaluates role / policy requirements
     │
[6] UseAuditApiCallLogging       — Captures HTTP call to async audit queue (non-blocking)
     │
[7] UseOutputCache               — Returns Redis-cached responses for eligible endpoints
     │
     ▼
[Endpoint Routing]
```

### Bypassed paths (no tenant resolution, no auth required)

| Path | Reason |
|------|--------|
| `/health/*` | Infrastructure probes — no tenant context |
| `/connect/*` | OpenIddict token endpoints — anonymous by design |
| `/api/v1/auth/register` | Account creation — user does not yet have a JWT |
| `/api/auth/sso/*` | Browser-redirect OAuth flows |
| `/openapi/*` | API spec JSON (development only) |
| `/scalar/*` | Scalar UI (development only) |

---

## OpenAPI / Scalar

| URL | Content |
|-----|---------|
| `/openapi/v1.json` | OpenAPI 3.1 JSON spec (development only) |
| `/scalar/v1` | Scalar interactive API reference |

The spec is generated by .NET 10's native `Microsoft.AspNetCore.OpenApi` package.

**JWT in OpenAPI spec:** `BearerSecuritySchemeTransformer` is **deferred** (OpenAPI v2 API changes). Scalar auth uses `WithAuthentication` in `OpenApiExtensions.cs` — paste the JWT from `POST /connect/token` manually in the Authorize dialog.

### Using Scalar in development

1. Start the API (Kestrel `:5000` or Docker `:8080`).
2. Navigate to `/scalar/v1` (e.g. `http://localhost:5000/scalar/v1`).
3. Click **Authorize**, paste the Bearer token from `POST /connect/token`.
4. Call versioned endpoints under `/api/v1/...`.

---

## Health Checks

| Path | Probe type | Checks |
|------|-----------|--------|
| `/health/live` | Liveness | Process is up |
| `/health/ready` | Readiness | PostgreSQL, Redis, MongoDB |

Kubernetes `livenessProbe` should point to `/health/live`.  
`readinessProbe` should point to `/health/ready`.

---

## Environment Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ashraak;Username=postgres;Password=...",
    "Redis": "localhost:6379",
    "MongoDB": "mongodb://localhost:27017"
  },
  "Seq": {
    "Url": "http://localhost:5341"
  },
  "Authentication": {
    "Google": { "ClientId": "", "ClientSecret": "" },
    "Microsoft": { "ClientId": "", "ClientSecret": "" }
  }
}
```

---

## Key Package Dependencies (host-level)

| Package | Purpose |
|---------|---------|
| `Asp.Versioning.Http` | URL-segment API versioning for Minimal APIs |
| `Microsoft.AspNetCore.OpenApi` | Native OpenAPI 3.1 document generation |
| `Scalar.AspNetCore` | Interactive API reference UI |
| `Serilog.AspNetCore` | Structured HTTP request logging |
| `OpenTelemetry.*` | Distributed tracing + metrics (OTLP export) |
| `AspNetCore.HealthChecks.*` | Postgres / Redis / MongoDB probes |

---

## Changes Made in This Phase

| File | Change |
|------|--------|
| `Directory.Packages.props` | Added `Asp.Versioning.Http 9.1.0` |
| `Ashraak.Api.csproj` | Added `Asp.Versioning.Http` reference |
| `Extensions/ModuleExtensions.cs` | **NEW** — `AddModules`, `MapModules`, `MapModuleProtocolEndpoints` |
| `Extensions/OpenApiExtensions.cs` | **NEW** — `AddOpenApiDocs`, `MapOpenApiDocs`, JWT security scheme |
| `Program.cs` | **REWRITTEN** — versioned groups, cleaner sections, bootstrap logger, health check tags |
| `Auth/Endpoints/AuthEndpoints.cs` | Removed `/api/` prefix; split into `MapAuthEndpoints` (versioned) + `MapAuthProtocolEndpoints` (unversioned) |
| `Tenant/Endpoints/TenantEndpoints.cs` | Removed `/api/` prefix from route group |
| `Users/Endpoints/UserEndpoints.cs` | Removed `/api/` prefix from route group |
| `Audit/Endpoints/AuditEndpoints.cs` | Removed `/api/` prefix from route group |
