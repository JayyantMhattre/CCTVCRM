# Building Blocks — Registration

There is **no central** `AddBuildingBlocks()` extension in the host. Building Blocks types are project-referenced and used selectively.

## What is registered today

### Per-module MediatR

Each module registers its own handlers:

```csharp
// Example: TenantModule.cs
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ProvisionTenantCommand).Assembly));
```

This picks up `ICommandHandler`, `IQueryHandler`, and `INotificationHandler` in the module Application assembly.

### Per-module FluentValidation

Auth and Tenant:

```csharp
services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);
```

Validation runs only if a pipeline behavior is registered — BuildingBlocks `ValidationBehavior` is **not** registered. Validators exist but may not execute automatically unless invoked manually.

### EF interceptors (cross-module)

Audit registers:

```csharp
services.AddSingleton<IInterceptor, AuditEntityChangeInterceptor>();
```

Tenant and Users DbContexts resolve all `IInterceptor` services at context configuration time.

## What is NOT registered

| Component | File | Status |
|-----------|------|--------|
| `ValidationBehavior` | `Application/Behaviors/ValidationBehavior.cs` | Not in DI pipeline |
| `LoggingBehavior` | `Application/Behaviors/LoggingBehavior.cs` | Not in DI pipeline |
| `PerformanceBehavior` | `Application/Behaviors/PerformanceBehavior.cs` | Not in DI pipeline |
| `ValidationPipelineBehavior` | Tenant.Application | Not in DI pipeline |
| `IEventBus` → `InProcessEventBus` | `EventBus/InProcessEventBus.cs` | No registration |
| `BaseDbContext` | Infrastructure | Modules don't inherit it |
| `OutboxProcessorBase` subclass | Infrastructure | No concrete job |
| Quartz hosting | — | Not in `Program.cs` |
| `SqlDataModule.AddSqlDataLayer` | Data.Sql | Not called |
| `MongoDataModule.AddMongoDataLayer` | Data.Mongo | Not called |

## Project references

| Consumer | References |
|----------|------------|
| `Ashraak.Api` (host) | Application, Infrastructure (transitive) |
| Auth, Tenant, Users, Audit Application | Application |
| Auth, Tenant, Users Infrastructure | Infrastructure |
| All modules | SharedKernel |

## Enabling pipeline behaviors (manual)

To register BuildingBlocks behaviors globally in the host:

```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(/* assemblies */);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
});
```

Not implemented today — modules rely on explicit validation in handlers or unregistered FluentValidation.

## Enabling outbox processor (manual)

1. Subclass `OutboxProcessorBase` for each schema or use a shared connection.
2. Register Quartz in host:

```csharp
services.AddQuartz(q => q.AddJob<YourOutboxProcessor>(...));
services.AddQuartzHostedService();
```

3. Make module DbContexts inherit `BaseDbContext`.

See [Extending](./extending.md).

## Enabling event bus (manual)

```csharp
services.AddSingleton<IEventBus, InProcessEventBus>();
// Future: services.AddSingleton<IEventBus, MassTransitEventBus>();
```

No callers exist yet — modules publish via `IPublisher` directly.
