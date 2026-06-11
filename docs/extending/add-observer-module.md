# Add an Observer Module

Use **Audit** as the reference implementation for Layer 3 cross-cutting modules.

---

## Characteristics

- No business aggregates other modules depend on
- Observes via middleware, interceptors, MediatR handlers
- Exposes contract `IYourObserverService` in SharedKernel.Contracts
- Registers **after** all observed modules

---

## Integration patterns

| Pattern | Audit example |
|---------|---------------|
| HTTP middleware | `AuditApiCallMiddleware` |
| EF interceptor | `AuditEntityChangeInterceptor` implements `SaveChangesInterceptor` |
| Domain events | `DomainEventAuditHandler : INotificationHandler<IDomainEvent>` |
| Async write | `Channel` + `BackgroundService` |

Register interceptor for other modules:

```csharp
options.AddInterceptors(sp.GetServices<IInterceptor>());
```

Modules resolve `IInterceptor` from DI without referencing Audit assembly.

---

## Registration

```csharp
// ModuleExtensions — last
services.AddYourObserverModule(configuration);
routeBuilder.MapYourObserverEndpoints();

// Program.cs — after auth, before output cache
app.UseYourObserverMiddleware();
```

---

## Disable observer

1. Comment `Add*` and `Map*` in `ModuleExtensions`
2. Remove `ProjectReference` from Api csproj
3. Remove middleware from `Program.cs`
4. Remove health checks that depend on observer's datastore

See DEVELOPER_GUIDE §4 (Audit disable example).

---

## Documentation required

Full `docs/modules/{observer}/` per governance, emphasizing:

- What is captured
- Performance impact
- Excluded paths
- Storage and retention

---

## Related

- [modules/audit/README.md](../modules/audit/README.md)
- [ADR-0003](../adr/ADR-0003-observer-modules.md)
