# Audit — Events

Audit module **consumes** events for logging — it does **not publish** domain or integration events.

## Consumed: all IDomainEvent

**Handler:** `DomainEventAuditHandler`  
**File:** `Ashraak.Audit.Application/EventHandlers/DomainEventAuditHandler.cs`

```csharp
public sealed class DomainEventAuditHandler : INotificationHandler<IDomainEvent>
```

| Behavior | Detail |
|----------|--------|
| Trigger | Any MediatR-published `IDomainEvent` |
| Module name | Parsed from event type namespace (segment 2) |
| TenantId/UserId | Reflection on event properties if present |
| Action | Event type name |
| Output | `IAuditService.LogAsync` → MongoDB queue |

### Examples captured when published

| Event | Source module |
|-------|---------------|
| `UserRegisteredEvent` | Auth |
| `TenantCreatedDomainEvent` | Only if published via MediatR (not today) |
| Any future contract event | Owning module |

**Note:** Domain events on aggregates that are never published via MediatR are **not** captured by this handler — only EF interceptor captures the entity change.

## Consumed: EF entity changes (not MediatR)

`AuditEntityChangeInterceptor` audits `Added`, `Modified`, `Deleted` — independent of domain event publication.

## Consumed: HTTP requests (not MediatR)

`AuditApiCallMiddleware` logs API calls — independent of domain events.

## Not consumed

| Type | Reason |
|------|--------|
| `IIntegrationEvent` | No integration event bus wired |
| Outbox rows | No outbox processor |
| RabbitMQ messages | Not connected |

## IAuditService as integration point

Modules can audit custom actions without events:

```csharp
await _auditService.LogAsync(new AuditEntryDto(
    tenantId, userId, "MyModule", "CustomAction", ...));
```

Inject `IAuditService` from SharedKernel.Contracts — implemented by `AuditRepository`.

## Event ordering

Audit writes are **async** via channel — no guarantee of strict ordering relative to HTTP response. Hash chain provides per-tenant ordering verification on read (Phase 2).

## Exclusions

EF interceptor skips `OutboxMessage` entities to avoid auditing outbox infrastructure rows.

Middleware skips audit log endpoint itself to prevent recursion.

## Related

- [Shared Kernel events](../shared-kernel/events.md) — contract event catalog
- [Architecture sequence diagram](./architecture.md#end-to-end-sequence-request-with-ef-save)
