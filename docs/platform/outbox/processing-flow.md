# Outbox — Processing Flow

```mermaid
sequenceDiagram
    participant Handler as Command Handler
    participant DB as Module DbContext
    participant Outbox as outbox_messages
    participant Worker as OutboxProcessorHostedService
    participant MediatR
    participant Consumer as Event Handlers

    Handler->>DB: SaveChangesAsync
    Note over DB,Outbox: Serialize domain events to outbox (same transaction)
    Worker->>Outbox: Poll unprocessed batch
    Worker->>MediatR: Publish deserialized notification
    MediatR->>Consumer: Handle (bridges, Users, Notifications, Audit)
    Worker->>Outbox: Mark ProcessedOnUtc / Error
```

## Registration flow change

`RegisterUserCommandHandler` no longer calls `IPublisher.Publish` directly — `UserRegisteredDomainEvent` is raised on the aggregate and written to outbox on save.
