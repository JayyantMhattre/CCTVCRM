# Building Blocks — Extending

## Enable global MediatR pipeline behaviors

Add to host `Program.cs` after module registration:

```csharp
services.AddMediatR(cfg =>
{
    // Register all module assemblies
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ProvisionTenantCommand).Assembly);
    // ... other modules

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
});
```

Alternatively, register Tenant's `ValidationPipelineBehavior` for Result-based validation failures.

## Wire outbox end-to-end

### 1. Inherit BaseDbContext

```csharp
public sealed class TenantDbContext : BaseDbContext
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }
    // existing DbSets...
}
```

Remove duplicate `OutboxMessage` configuration if base handles it.

### 2. Create OutboxProcessor

```csharp
public sealed class TenantOutboxProcessor : OutboxProcessorBase
{
    public TenantOutboxProcessor(
        TenantDbContext dbContext,
        IPublisher publisher,
        ILogger<TenantOutboxProcessor> logger)
        : base(dbContext, publisher, logger) { }
}
```

### 3. Register Quartz in host

```csharp
services.AddQuartz(q =>
{
    q.AddJob<TenantOutboxProcessor>(opts => opts
        .WithIdentity("tenant-outbox")
        .WithCronSchedule("0/10 * * * * ?")); // every 10s
});
services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
```

Repeat per schema or use a single processor with multiple DbContexts.

### 4. Map domain events to contract events

Either in the processor after deserialization, or via dedicated `INotificationHandler` for domain events within the same module.

## Enable IEventBus

Phase 1 (no broker):

```csharp
services.AddSingleton<IEventBus, InProcessEventBus>();
```

Phase 3 (MassTransit + RabbitMQ):

1. Add MassTransit packages to EventBus project
2. Implement `MassTransitEventBus : IEventBus`
3. Configure RabbitMQ from `ConnectionStrings:RabbitMQ` or env vars
4. Replace DI registration — callers use `IEventBus` instead of direct `IPublisher` for cross-boundary events

Keep MediatR for in-process command/query; use `IEventBus` for cross-service integration.

## Adopt Data.Sql layer

For a new read model or greenfield module:

```csharp
services.AddSqlDataLayer<MyReadDbContext>(configuration);
services.AddScoped<IDataRepository<MyEntity>, GenericSqlRepository<MyEntity>>();
```

Configure `Database:Provider` in appsettings (`PostgreSql`, `SqlServer`, etc.).

**Note:** Existing modules (Auth, Tenant, Users) use DDD DbContexts — migrating requires careful boundary analysis. See `DATA_LAYER.md`.

## Adopt Data.Mongo layer

Replace hand-rolled Mongo in Audit:

```csharp
services.AddMongoDataLayer(configuration);
services.AddMongoRepository<AuditEntry>();
```

Audit currently uses `AuditModule.cs` with direct `IMongoClient` registration — equivalent but not using BuildingBlocks Data.Mongo.

## Add a new BuildingBlocks project

Follow naming: `Ashraak.BuildingBlocks.{Concern}`. Add to `Ashraak.slnx`. Reference from host or modules as needed. Do not create circular references to module projects.
