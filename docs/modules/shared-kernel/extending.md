# Shared Kernel — Extending

Guidelines for adding new cross-module contracts without breaking existing modules.

## Adding a contract event

1. Create a record in `BackEnd/src/Shared/Ashraak.SharedKernel.Contracts/{Module}/Events/`:

```csharp
public sealed record MyNewEvent(Guid TenantId, Guid EntityId) : DomainEvent;
```

2. Publish from the owning module's command handler **after** `SaveChangesAsync`:

```csharp
await _publisher.Publish(new MyNewEvent(tenantId, entityId), cancellationToken);
```

3. Add `INotificationHandler<MyNewEvent>` in consuming module's Application project.

4. Register MediatR in the consuming module (already done if handlers are in the registered assembly).

**Do not** reference another module's Domain project from your module — use Contracts only.

## Adding a cross-module interface

1. Define interface + DTOs in `Ashraak.SharedKernel.Contracts/{Module}/Interfaces/` and `Dtos/`.
2. Implement in the owning module's Infrastructure project.
3. Register in the module's `*Module.cs`:

```csharp
services.AddScoped<IMyService, MyService>();
```

4. Inject `IMyService` in other modules via constructor DI (Application or Infrastructure layer only).

## Adding a domain primitive

Prefer extending existing types in `Ashraak.SharedKernel/Domain/`. Changes here affect every module — keep additions minimal and backward-compatible.

For module-specific value objects, put them in the module's Domain project, not Shared Kernel.

## Outbox promotion (future)

When wiring outbox processing:

1. Make module DbContext inherit `BaseDbContext` (BuildingBlocks.Infrastructure).
2. Configure `OutboxMessage` in `OnModelCreating` (copy from `BaseDbContext`).
3. Create a concrete `OutboxProcessorBase` subclass per schema or a shared processor.
4. Map domain events to contract events in the processor or via dedicated mapper classes.

Until then, direct `IPublisher.Publish` after save is the supported pattern.

## Versioning contract events

- Add new properties as optional with defaults, or create a new event type.
- Do not rename or remove properties on published events without a migration plan.
- Event `Type` in outbox uses assembly-qualified names — renaming breaks deserialization.

## Project reference rules

```
✅ Module.Infrastructure → SharedKernel.Contracts
✅ Module.Application   → SharedKernel.Contracts
✅ Module.Domain        → SharedKernel (primitives only)
❌ SharedKernel         → any module
❌ SharedKernel.Contracts → any module
❌ Module A.Domain      → Module B.Domain
```

Architecture tests in `BackEnd/tests/Ashraak.Architecture.Tests/` enforce layer boundaries.

## Checklist for new contract

- [ ] Event/interface in Contracts project
- [ ] Implementation in owning Infrastructure
- [ ] DI registration in `*Module.cs`
- [ ] Handler(s) in consuming Application projects
- [ ] Audit will auto-capture if event extends `DomainEvent`
- [ ] Document in module's [events.md](../) if externally visible
