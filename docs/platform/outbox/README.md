# Outbox Platform

Reliable in-process event dispatch for Auth, Tenant, and Users SQL modules.

## Components

| Type | Location |
|------|----------|
| `OutboxMessage` | SharedKernel |
| `BaseDbContext` / serializer | BuildingBlocks.Infrastructure |
| `OutboxProcessorHostedService<T>` | BuildingBlocks.Infrastructure |
| Host registration | `OutboxExtensions.AddOutboxProcessors` |

## Docs

- [architecture.md](./architecture.md)
- [processing-flow.md](./processing-flow.md)
- [failure-recovery.md](./failure-recovery.md)
- [retry-behavior.md](./retry-behavior.md)
- [operations.md](./operations.md)
