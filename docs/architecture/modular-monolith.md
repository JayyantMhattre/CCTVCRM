# Modular Monolith

Ashraak uses a **modular monolith**: one deployable API process with **strict module boundaries** enforced by project structure and architecture tests.

---

## Composition root

`Ashraak.Api` is the only executable host. It:

- Registers all modules via `ModuleExtensions.AddModules()`
- Maps all HTTP routes via `MapModules()` and `MapModuleProtocolEndpoints()`
- Owns the middleware pipeline (`Program.cs`)
- Provides host adapters: `ICurrentUser`, `ITenantContext`, `IDateTimeProvider`

Modules never reference each other’s **Infrastructure** or **Api** projects.

---

## Module shape

Each feature module follows vertical slice layering:

```
Module/
├── {Product}.Module.Domain          # Aggregates, domain events, repository interfaces
├── {Product}.Module.Application     # Commands, queries, validators, handlers
├── {Product}.Module.Infrastructure  # EF/Mongo, DI module class, repositories
└── {Product}.Module.Api             # Minimal API endpoints, middleware (if any)
```

**Registration contract** (every module):

```csharp
services.Add{Module}Module(configuration);
routeBuilder.Map{Module}Endpoints();
```

---

## Registration layers

```
Layer 0: Caching     → ICacheService (required by Auth)
Layer 1: Auth        → Identity, OpenIddict, permissions
Layer 2: Tenant, Users → Business data + contract services
Layer 3: Audit       → Observers (interceptors, middleware, event handlers)
```

Order is defined in `BackEnd/src/Host/Ashraak.Api/Extensions/ModuleExtensions.cs`.

---

## Cross-module integration

| Allowed | Mechanism |
|---------|-----------|
| Yes | `Ashraak.SharedKernel.Contracts` interfaces (`ITenantService`, `IUserService`, …) |
| Yes | Contract domain events (`UserRegisteredEvent`, …) via MediatR |
| Yes | `IInterceptor` resolved from DI (Audit attaches to all DbContexts) |
| No | Direct project reference to another module’s Infrastructure |
| No | Shared DbContext across modules |

---

## PostgreSQL schema isolation

| Module | Schema |
|--------|--------|
| Auth | `auth` |
| Tenant | `tenant` |
| Users | `users` |

Each module has its own `DbContext`, migrations history table, and connection string key (with `DefaultConnection` fallback).

---

## Observer pattern (Audit)

Audit is a **Layer 3 observer**:

- Does not own business transactions
- Captures side effects: HTTP calls, EF changes, domain events
- Writes asynchronously to MongoDB via an internal channel + background service

Future notification/webhook modules should follow the same layer.

---

## Disabling a module

Comment out `Add{Module}` and `Map{Module}` in `ModuleExtensions.cs`, remove `ProjectReference` from `Ashraak.Api.csproj`, and remove dependent middleware/health checks. See [DEVELOPER_GUIDE.md](../../DEVELOPER_GUIDE.md) §4.

---

## Related

- [Module map](./module-map.md)
- [Dependency rules](./dependency-rules.md)
- [ADR-0001](../adr/ADR-0001-modular-monolith.md)
