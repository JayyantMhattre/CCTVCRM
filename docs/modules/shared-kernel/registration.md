# Shared Kernel — Registration

The Shared Kernel projects contain **no DI extension methods**. Registration happens in the host and individual modules.

## Host registrations (`Program.cs`)

File: `BackEnd/src/Host/Ashraak.Api/Program.cs`

| Abstraction | Implementation | Lifetime |
|-------------|----------------|----------|
| `ICurrentUser` | `CurrentUser` | Scoped |
| `ITenantContext` | `TenantContext` | Scoped |
| `IDateTimeProvider` | `DateTimeProvider` | Singleton |

These are registered **before** `AddModules(configuration)`.

## Module registrations

Each SQL module aliases its DbContext as `IUnitOfWork`:

```csharp
// Pattern in AuthModule.cs, TenantModule.cs, UsersModule.cs
services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AuthDbContext>());
```

| Interface | Implementation | Module |
|-----------|----------------|--------|
| `ITenantService` | `TenantService` | Tenant.Infrastructure |
| `IUserService` | `UserService` | Users.Infrastructure |
| `IAuthPermissionChecker` | `AuthPermissionChecker` | Auth.Infrastructure |
| `IAuditService` | `AuditRepository` | Audit.Infrastructure |
| `IDomainEventPublisher` | `DomainEventPublisher` | Auth.Infrastructure (only module) |

## MediatR contract event handlers

Handlers are registered per-module via `AddMediatR`:

```csharp
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SomeCommand).Assembly));
```

Example: `UserRegisteredEventHandler` in Users.Application is discovered when Users module registers MediatR from the Users Application assembly.

## What is not registered

| Item | Status |
|------|--------|
| SharedKernel types directly | No — they are base classes/interfaces, not services |
| `IDomainEventPublisher` globally | Only Auth registers it |
| Outbox processor | No concrete `OutboxProcessorBase` subclass registered |
| `IEventBus` | Not registered anywhere |

## Project references

Every module Application and Infrastructure project references:

- `Ashraak.SharedKernel`
- `Ashraak.SharedKernel.Contracts` (where cross-module calls are needed)

Host references SharedKernel directly for health checks and cross-cutting types.

## Configuration keys

Shared Kernel types do not read configuration. Runtime abstractions consume HTTP context and claims set by Auth middleware — see [Host registration](../host/registration.md).
