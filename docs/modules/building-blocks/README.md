# Building Blocks

Reusable infrastructure patterns for Ashraak modules: CQRS markers, MediatR pipeline behaviors, EF persistence helpers, outbox processor scaffold, event bus abstraction, and optional multi-provider data layer.

**Source:** `BackEnd/src/BuildingBlocks/`

**EF Core version:** 9.0.4 (central package management, .NET 10 host)

## Projects

| Project | Path | Status |
|---------|------|--------|
| `Ashraak.BuildingBlocks.Application` | `BuildingBlocks/Ashraak.BuildingBlocks.Application/` | Referenced by modules; behaviors not globally registered |
| `Ashraak.BuildingBlocks.EventBus` | `BuildingBlocks/Ashraak.BuildingBlocks.EventBus/` | Scaffold — `IEventBus` not registered |
| `Ashraak.BuildingBlocks.Infrastructure` | `BuildingBlocks/Ashraak.BuildingBlocks.Infrastructure/` | `BaseDbContext` unused by module DbContexts |
| `Ashraak.BuildingBlocks.Data.Abstractions` | `BuildingBlocks/Ashraak.BuildingBlocks.Data.Abstractions/` | Not wired into host |
| `Ashraak.BuildingBlocks.Data.Sql` | `BuildingBlocks/Ashraak.BuildingBlocks.Data.Sql/` | Not wired into host |
| `Ashraak.BuildingBlocks.Data.Mongo` | `BuildingBlocks/Ashraak.BuildingBlocks.Data.Mongo/` | Not wired — Audit uses hand-rolled Mongo |

## Key facts

- **Outbox:** `BaseDbContext` serializes domain events to `OutboxMessage` on save; **no module DbContext inherits it today**
- **Outbox processor:** `OutboxProcessorBase` (Quartz) exists; **no concrete subclass or Quartz hosting**
- **RabbitMQ:** Docker-compose only; **MassTransit not referenced**; `InProcessEventBus` is a log-only stub
- **Pipeline behaviors:** `ValidationBehavior`, `LoggingBehavior`, `PerformanceBehavior` exist but are **not registered** in host or modules (Tenant has its own `ValidationPipelineBehavior`, also unregistered)

## Module documentation

- [Architecture](./architecture.md) — CQRS, persistence, outbox, data layer design
- [Registration](./registration.md) — what is and is not wired today
- [API](./api.md) — public types reference
- [Events](./events.md) — integration events vs domain events
- [Extending](./extending.md) — enabling outbox, event bus, data layer
- [Operations](./operations.md) — EF version, Quartz, future RabbitMQ

## Related modules

- [Shared Kernel](../shared-kernel/README.md) — `OutboxMessage`, domain events
- [Host](../host/README.md) — composition root (does not call a central `AddBuildingBlocks()`)
