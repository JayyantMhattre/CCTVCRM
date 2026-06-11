# Tenant — Registration

Entry point: `BackEnd/src/Modules/Tenant/Ashraak.Tenant.Infrastructure/TenantModule.cs`

Host order: Layer 2 (after Auth, before Users).

## DbContext

```csharp
services.AddDbContext<TenantDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__ef_migrations_history", "tenant");
        npgsql.EnableRetryOnFailure(3);
    });
    options.AddInterceptors(sp.GetServices<IInterceptor>());
});
```

Connection: `ConnectionStrings:Tenant` → fallback `ConnectionStrings:DefaultConnection`

## Services

| Registration | Lifetime |
|--------------|----------|
| `IUnitOfWork` → `TenantDbContext` | Scoped |
| `ITenantRepository` → `TenantRepository` | Scoped |
| `ITenantService` → `TenantService` | Scoped |

## MediatR and FluentValidation

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProvisionTenantCommand).Assembly));
services.AddValidatorsFromAssembly(typeof(ProvisionTenantCommand).Assembly);
```

**Gap:** `ValidationPipelineBehavior<TRequest,TResponse>` in Application is **not** added to MediatR pipeline.

## Endpoint registration

```csharp
// ModuleExtensions.MapModules
routeBuilder.MapTenantEndpoints();
```

Maps group `/tenants` under host version prefix → `/api/v1/tenants/*`

## Configuration keys

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:Tenant` | PostgreSQL, schema `tenant` |
| `ConnectionStrings:DefaultConnection` | Fallback |
| `ConnectionStrings:Redis` | Indirect — TenantService cache |
| `ASPNETCORE_ENVIRONMENT` | Cache key prefix via Caching module |

## Cross-module consumption

Other modules inject `ITenantService` — no direct reference to Tenant.Infrastructure.

Registered consumers: Auth (token, register), Auth middleware (active check), Users endpoints.
