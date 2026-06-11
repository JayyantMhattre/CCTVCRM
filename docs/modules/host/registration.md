# Host — Registration

## Host-level DI (`Program.cs`)

File: `BackEnd/src/Host/Ashraak.Api/Program.cs`

Registered **before** modules:

| Service | Implementation | Lifetime |
|---------|----------------|----------|
| `IHttpContextAccessor` | framework | Singleton |
| `ICurrentUser` | `CurrentUser` | Scoped |
| `ITenantContext` | `TenantContext` | Scoped |
| `IDateTimeProvider` | `DateTimeProvider` | Singleton |
| `IExceptionHandler` | `GlobalExceptionHandler` | — |
| ProblemDetails | framework | — |

### Authentication and authorization

```csharp
builder.Services.AddAuthentication();  // empty — schemes added by Auth module
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TenantAdmin", policy => policy.RequireRole("Admin", "Manager"));
});
```

OpenIddict validation is registered inside `AddAuthModule`.

### Platform hardening (`AddHostPlatformServices`)

After `AddModules` / `AddOutboxProcessors`:

| Service | Implementation | Notes |
|---------|----------------|-------|
| `RateLimitingOptions` | `IOptions` from `RateLimiting` section | Redis middleware |
| `FeatureFlagOptions` | `IOptions` from `Features` section | Config flags |
| `IFeatureFlagService` | `ConfigFeatureFlagService` | Singleton |
| Health checks | `NotificationsHealthCheck`, `OutboxProcessorsHealthCheck` | Chained on `AddHealthChecks()` |

Startup: `builder.ValidateAshraakEnvironment()` before `Build()`.

### Output cache

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(30)));
});
builder.Services.AddStackExchangeRedisOutputCache(/* ConnectionStrings:Redis */);
```

### API versioning

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(/* ... */);
```

### OpenTelemetry

Service name `Ashraak.Api`, OTLP exporter configured from environment.

### Health checks

| Check | Tags | Path |
|-------|------|------|
| Self (live) | `live` | `/health/live` |
| PostgreSQL | `ready` | `/health/ready` |
| Redis | `ready` | `/health/ready` |
| MongoDB | `ready` | `/health/ready` |

### OpenAPI (Development)

`AddOpenApiDocs()` / `MapOpenApiDocs()` — `/openapi/v1.json`, `/scalar/v1`

## Module registration

File: `BackEnd/src/Host/Ashraak.Api/Extensions/ModuleExtensions.cs`

```csharp
public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
{
    services.AddCachingModule(configuration);   // Layer 0
    services.AddAuthModule(configuration);    // Layer 1
    services.AddTenantModule(configuration);  // Layer 2
    services.AddUsersModule(configuration);   // Layer 2
    services.AddAuditModule(configuration);   // Layer 3
    return services;
}
```

## Endpoint registration

```csharp
// Unversioned — before versioned group
app.MapModuleProtocolEndpoints();  // → MapAuthProtocolEndpoints()

var v1 = app.MapGroup("/api/v{version:apiVersion}")
    .HasApiVersion(1, 0);
v1.MapModules();  // Auth, Tenant, Users, Audit REST
```

## Project references (`Ashraak.Api.csproj`)

- `Ashraak.SharedKernel`
- `Ashraak.BuildingBlocks.*` (Application, Infrastructure — transitive)
- Each module: `*.Infrastructure` + `*.Api`
- `Ashraak.Caching.Redis`

## Configuration binding

Host reads standard `IConfiguration` (appsettings + env vars). Module-specific keys are consumed inside each `*Module.cs` — see per-module [registration.md](../) files.

Key host-level keys:

| Key | Used by |
|-----|---------|
| `ConnectionStrings:DefaultConnection` | Health checks |
| `ConnectionStrings:Redis` | Output cache, Caching module |
| `Seq:Url` | Serilog |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry (env) |

## What is not registered in host

| Item | Notes |
|------|-------|
| `AddBuildingBlocks()` | No central extension |
| MediatR pipeline behaviors | Per-module only |
| Quartz / outbox processor | Not hosted |
| `IEventBus` | Not registered |
| CORS | Not configured |
| Static files / SPA | React served separately |
