# Building Blocks — Events

Two parallel event systems exist in Ashraak. Only one is active today.

## Active: MediatR domain/contract events

**Location:** Shared Kernel and SharedKernel.Contracts

- Extend `DomainEvent` → `IDomainEvent` → `INotification`
- Dispatched in-process via `IPublisher.Publish`
- Handlers: `INotificationHandler<TEvent>`
- Cross-module example: `UserRegisteredEvent` → `UserRegisteredEventHandler`

See [Shared Kernel events](../shared-kernel/events.md).

## Scaffold: Integration events (EventBus)

**Location:** `BackEnd/src/BuildingBlocks/Ashraak.BuildingBlocks.EventBus/`

| Type | Purpose |
|------|---------|
| `IIntegrationEvent` | Marker for events destined for a message broker |
| `IntegrationEvent` | Abstract base with `EventId`, `OccurredOnUtc` |
| `IEventBus.PublishAsync<TEvent>` | Abstraction over transport |
| `InProcessEventBus` | Stub — logs event type and ID, no persistence |

### InProcessEventBus behavior

```csharp
// Publishes nothing to MediatR or outbox — logs only
_logger.LogInformation("Publishing integration event {EventType} ({EventId})", ...);
return Task.CompletedTask;
```

**Not registered in DI.** No code calls `IEventBus` today.

## Outbox events (Infrastructure)

**Location:** `OutboxMessage` in SharedKernel, serialization in `BaseDbContext`

Designed flow:

1. Aggregate raises `IDomainEvent`
2. `BaseDbContext.SaveChangesAsync` writes to `outbox_messages` (same DB transaction)
3. `OutboxProcessorBase` (Quartz) reads pending rows, deserializes, publishes to MediatR
4. Marks row processed or failed

**Status:** Steps 1–2 inactive (modules don't inherit `BaseDbContext`). Step 3 has no concrete processor.

## RabbitMQ / MassTransit

| Item | Status |
|------|--------|
| RabbitMQ in `docker-compose.yml` | Present for future use |
| MassTransit package | Not referenced |
| `IEventBus` broker implementation | Not implemented |
| Contract events as integration events | Not mapped |

Phase 3 plan: register MassTransit-backed `IEventBus` without changing callers. Contract events would need explicit mapping to `IIntegrationEvent` types.

## Event type comparison

| Aspect | DomainEvent (MediatR) | IIntegrationEvent (EventBus) |
|--------|----------------------|------------------------------|
| Base type | `DomainEvent` | `IntegrationEvent` |
| Dispatch | `IPublisher` | `IEventBus` (future) |
| Handlers | `INotificationHandler<T>` | Consumer classes (future) |
| Persistence | Outbox scaffold (unused) | Broker (not wired) |
| Cross-process | No | Yes (when wired) |
| Used today | Yes | No |

## Audit observation

Audit's `DomainEventAuditHandler` subscribes to `IDomainEvent` — it captures MediatR-published events only, not integration events or undispatched outbox rows.
