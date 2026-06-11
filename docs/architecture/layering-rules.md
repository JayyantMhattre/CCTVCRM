# Layering Rules

## Per-module layers

| Layer | Contains | References |
|-------|----------|------------|
| **Domain** | Aggregates, value objects, domain events, repository interfaces | SharedKernel only |
| **Application** | Commands, queries, handlers, validators, module-specific behaviors | Domain, SharedKernel, Contracts (events only) |
| **Infrastructure** | DbContext, repositories, module DI (`*Module.cs`), external adapters | Application, Domain, BuildingBlocks.Infrastructure, Contracts |
| **Api** | Minimal API endpoints, middleware extensions | Application, Infrastructure (for DI extension), Contracts |

**Api must not** contain business logic — only HTTP mapping, auth attributes, and MediatR dispatch.

---

## Shared layers

### SharedKernel

Domain primitives and host abstractions used by all modules:

- `AggregateRoot`, `Entity<T>`, `ValueObject`
- `IDomainEvent`, `Result`, `Error`
- `ICurrentUser`, `ITenantContext`, `IUnitOfWork`
- `OutboxMessage` (persistence model)

### SharedKernel.Contracts

**Only** cross-module integration surface:

- Service interfaces (`ITenantService`, `IUserService`, `IAuthPermissionChecker`, `IAuditService`)
- Integration-oriented events (`UserRegisteredEvent`, `TenantDeletedEvent`, …)
- DTOs shared across modules

No EF, no HTTP, no module-specific aggregates.

### BuildingBlocks

Reusable technical infrastructure — not business features:

- MediatR pipeline behaviors
- `BaseDbContext` (outbox serialization hook)
- `OutboxProcessorBase`
- Optional multi-provider data layer (SQL/Mongo abstractions)

---

## CQRS conventions

| Type | Marker | Handler |
|------|--------|---------|
| Command | `ICommand` / `IRequest<T>` | `ICommandHandler` / `IRequestHandler` |
| Query | `IQuery<T>` | `IQueryHandler` |

Validators: FluentValidation in Application assembly, registered per module in `*Module.cs`.

**Note:** Global `ValidationBehavior` / `LoggingBehavior` exist in BuildingBlocks but are **not registered** in module DI today — validation runs via FluentValidation direct invocation per handler setup.

---

## Host layer

`Ashraak.Api` references module **Infrastructure** and **Api** projects only.

Host-specific code:

- `Program.cs` — pipeline
- `Extensions/ModuleExtensions.cs` — module toggle point
- `Infrastructure/CurrentUser.cs` — `ICurrentUser` from `HttpContext`
- `Middleware/GlobalExceptionHandler.cs` — RFC 7807

---

## Frontend layering (React)

| Area | Path | Rule |
|------|------|------|
| Core | `src/core/` | Framework: API client, auth, router — no feature business rules |
| Modules | `src/modules/{feature}/` | Feature pages, `api.ts`, `types.ts` |
| Shared | `src/shared/` | Reusable UI, guards, hooks |
| Layouts | `src/layouts/` | Shell chrome only |

---

## Related

- [Dependency rules](./dependency-rules.md)
- [Extending — add backend module](../extending/add-backend-module.md)
