# Building Blocks — Operations

## EF Core version

Central management in `BackEnd/Directory.Packages.props`:

| Package | Version |
|---------|---------|
| Microsoft.EntityFrameworkCore | 9.0.4 |
| Microsoft.EntityFrameworkCore.Relational | 9.0.4 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.4 |

Comment in props: EF 9.x kept for Npgsql 9.x compatibility while app targets .NET 10.

Modules use PostgreSQL exclusively via `UseNpgsql` with `EnableRetryOnFailure(3)`.

## Quartz (outbox processor)

Package: Quartz 3.14.0 (referenced by Infrastructure project)

**Not hosted.** When enabled:

- Schedule outbox jobs per schema
- Monitor job failures via Serilog
- Set batch size in `OutboxProcessorBase` (default 20)

## RabbitMQ

Present in `BackEnd/docker-compose.yml` as infrastructure placeholder.

| Check | Command |
|-------|---------|
| Container running | `docker compose ps rabbitmq` |
| Management UI | Port 15672 (if exposed in compose) |

Application does not connect. No health check tag for RabbitMQ in host.

## Build verification

```powershell
dotnet build BackEnd/Ashraak.slnx
dotnet test BackEnd/tests/Ashraak.Architecture.Tests/
```

Architecture tests verify module layer boundaries against BuildingBlocks usage patterns.

## Deployment notes

Building Blocks assemblies ship embedded in module DLLs and the host — no separate deployment unit.

### Before enabling outbox in production

- [ ] Generate EF migrations for `outbox_messages` per schema
- [ ] Deploy Quartz hosted service
- [ ] Monitor outbox table for stuck rows (`ProcessedOnUtc IS NULL`)
- [ ] Alert on `Error` column population
- [ ] Test event deserialization after assembly version changes (assembly-qualified type names)

### Before enabling RabbitMQ

- [ ] Implement and register `IEventBus`
- [ ] Define integration event types separate from contract events (or explicit mappers)
- [ ] Add RabbitMQ health check to `/health/ready`
- [ ] Configure dead-letter queues and retry policies in MassTransit

## Troubleshooting

| Issue | Cause | Action |
|-------|-------|--------|
| Validators not running | Pipeline behaviors not registered | See [extending.md](./extending.md) |
| Outbox always empty | DbContext doesn't inherit BaseDbContext | Change inheritance or publish directly |
| Outbox rows never processed | No Quartz job | Register processor |
| `Type.GetType` fails in processor | Event type moved/renamed | Migration strategy for outbox Type column |
| `IEventBus` resolution fails | Not registered | Add DI registration |

## Related docs

- `BackEnd/src/BuildingBlocks/DATA_LAYER.md` — data layer design
- [Shared Kernel operations](../shared-kernel/operations.md) — outbox entity
- [Host operations](../host/operations.md) — health checks, connection strings
