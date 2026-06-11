# Shared Kernel

The shared kernel is the cross-cutting foundation for all Ashraak backend modules. It provides DDD primitives, result types, pagination, outbox entities, and runtime abstractions. Cross-module contracts live in a sibling project.

**Source:** `BackEnd/src/Shared/Ashraak.SharedKernel` and `BackEnd/src/Shared/Ashraak.SharedKernel.Contracts`

**Target framework:** .NET 10 (`Directory.Build.props`)

## Projects

| Project | Path | Purpose |
|---------|------|---------|
| `Ashraak.SharedKernel` | `BackEnd/src/Shared/Ashraak.SharedKernel/` | Domain primitives, outbox model, guards, results |
| `Ashraak.SharedKernel.Contracts` | `BackEnd/src/Shared/Ashraak.SharedKernel.Contracts/` | Cross-module interfaces, DTOs, contract events |

## What lives here

- **Domain layer:** `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, domain events
- **Outbox scaffold:** `OutboxMessage`, `IOutboxMessage` (serialization handled by BuildingBlocks `BaseDbContext`, not yet wired in module DbContexts)
- **Application abstractions:** `IUnitOfWork`, `ICurrentUser`, `ITenantContext`, `IDateTimeProvider`
- **Results:** `Result`, `Result<T>`, `Error`, `ErrorType`
- **Contracts:** `ITenantService`, `IUserService`, `IAuthPermissionChecker`, `IAuditService`, and MediatR contract events

## What does not live here

- DI registration (host and modules register implementations)
- EF Core DbContexts
- HTTP endpoints
- Redis, MongoDB, or RabbitMQ clients

## Module documentation

- [Architecture](./architecture.md) — domain model, events, outbox entity design
- [Registration](./registration.md) — how host and modules wire kernel abstractions
- [API](./api.md) — public types and contracts reference
- [Events](./events.md) — contract events and domain event interfaces
- [Extending](./extending.md) — adding new contract events and interfaces
- [Operations](./operations.md) — versioning, compatibility, and deployment notes

## Related modules

- [Building Blocks](../building-blocks/README.md) — CQRS behaviors, `BaseDbContext`, event bus scaffold
- [Host](../host/README.md) — registers `ICurrentUser`, `ITenantContext`, `IDateTimeProvider`
