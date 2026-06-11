# Users — Registration

Entry point: `BackEnd/src/Modules/Users/Ashraak.Users.Infrastructure/UsersModule.cs`

Host order: Layer 2 (after Tenant).

## DbContext

```csharp
services.AddDbContext<UsersDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__ef_migrations_history", "users");
        npgsql.EnableRetryOnFailure(3);
    });
    options.AddInterceptors(sp.GetServices<IInterceptor>());
});
```

Connection: `ConnectionStrings:Users` → fallback `ConnectionStrings:DefaultConnection`

`UsersDbContext` injects `ITenantContext` for global query filter.

## Services

| Registration | Lifetime |
|--------------|----------|
| `IUnitOfWork` → `UsersDbContext` | Scoped |
| `IUserProfileRepository` → `UserProfileRepository` | Scoped |
| `IUserService` → `UserService` | Scoped |

## MediatR

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserProfileCommand).Assembly));
```

Discovers:
- `CreateUserProfileCommandHandler`
- `UserRegisteredEventHandler`
- `TenantDeletedEventHandler`

No FluentValidation registered in Users module.

## Endpoint registration

```csharp
routeBuilder.MapUserEndpoints();  // → /users under /api/v1
```

## Configuration keys

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:Users` | PostgreSQL, schema `users` |
| `ConnectionStrings:DefaultConnection` | Fallback |

## Project references

- `Ashraak.Caching.Abstractions` (referenced; **no direct cache usage** in Infrastructure today)
- `Ashraak.SharedKernel`, `Ashraak.SharedKernel.Contracts`
- `Ashraak.BuildingBlocks.Application`

## Cross-module

Consumes `ITenantContext` from host. Does not reference Auth or Tenant Infrastructure projects.
