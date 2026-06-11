# Add Contracts and Handlers

Cross-module integration uses `Ashraak.SharedKernel.Contracts`.

---

## 1. Define contract

**Interface** (if other modules need to call you):

```
SharedKernel.Contracts/{Area}/Interfaces/IYourService.cs
```

**Event** (if other modules react to your action):

```
SharedKernel.Contracts/{Area}/Events/YourEvent.cs
```

Inherit `DomainEvent` from SharedKernel.

---

## 2. Implement interface

In your module Infrastructure:

```csharp
internal sealed class YourService : IYourService { }
```

Register in `YourModule.cs`:

```csharp
services.AddScoped<IYourService, YourService>();
```

---

## 3. Publish event

**Today (working pattern):**

```csharp
await publisher.Publish(new YourEvent(...), ct);
```

After `SaveChanges` in command handler.

**Future (outbox):** inherit `BaseDbContext` + processor — see [architecture/outbox.md](../architecture/outbox.md).

---

## 4. Consume event

In consumer Application layer:

```csharp
internal sealed class YourEventHandler : INotificationHandler<YourEvent>
{
    public async Task Handle(YourEvent notification, CancellationToken ct) { }
}
```

Register via `AddMediatR` in consumer's `*Module.cs`.

---

## 5. Document

Update:

- Publisher `docs/modules/{publisher}/events.md`
- Consumer `docs/modules/{consumer}/events.md`
- `docs/modules/shared-kernel/events.md` if catalog maintained

---

## Working example

| Event | Publisher | Consumer |
|-------|-----------|----------|
| `UserRegisteredEvent` | Auth `RegisterUserCommandHandler` | Users `UserRegisteredEventHandler` |

---

## Related

- [architecture/eventing.md](../architecture/eventing.md)
- [add-observer-module.md](./add-observer-module.md)
