# Dependency Rules

Enforced by convention and `Ashraak.Architecture.Tests`.

---

## Rules

### R1 — No module-to-module implementation references

```
❌ Ashraak.Users.Infrastructure → Ashraak.Auth.Infrastructure
✅ Ashraak.Users.Application → Ashraak.SharedKernel.Contracts
```

### R2 — Contracts are the integration API

Other modules depend on **interfaces and events** in `Ashraak.SharedKernel.Contracts`, implemented in Infrastructure:

| Interface | Implemented in |
|-----------|----------------|
| `ITenantService` | `Ashraak.Tenant.Infrastructure` |
| `IUserService` | `Ashraak.Users.Infrastructure` |
| `IAuthPermissionChecker` | `Ashraak.Auth.Infrastructure` |
| `IAuditService` | `Ashraak.Audit.Infrastructure` |

### R3 — Single DbContext per module

Each module owns one EF `DbContext` (except Audit → MongoDB only).

```
❌ Shared DbContext across Auth and Users
✅ AuthDbContext (schema auth) + UsersDbContext (schema users)
```

### R4 — Host is the composition root

Modules must not reference `Ashraak.Api`.

### R5 — Domain is innermost

Domain projects reference only `Ashraak.SharedKernel`.

### R6 — Observer modules are last

Audit registers `IInterceptor` and middleware that observe other modules without compile-time references to their Domain assemblies.

---

## Caching dependency

`Caching` must register **before** `Auth` because `AuthModule` resolves `ICacheService` during registration (permission cache, session cache).

---

## MediatR event dispatch

**In-process today:**

- `IDomainEventPublisher` / `IPublisher.Publish` for contract events
- `DomainEventAuditHandler` implements `INotificationHandler<IDomainEvent>` (Audit)

**Not in-process:**

- RabbitMQ / `IEventBus` — stub only

---

## Frontend dependencies

- Feature modules import from `@/` (`src/`) — core API and shared only
- Feature modules must **not** import from other feature modules’ pages (shared hooks/components OK)

---

## Violations to avoid when extending

| Anti-pattern | Why |
|--------------|-----|
| Importing `AuthUser` in Users module | Leaks auth domain |
| Querying `auth.users` from Users DbContext | Breaks schema isolation |
| Calling Audit Mongo from Auth | Bypasses `IAuditService` contract |
| Circular contract events | Unbounded handler chains |

---

## Related

- [Modular monolith](./modular-monolith.md)
- [Architecture tests](../../BackEnd/tests/Ashraak.Architecture.Tests/ArchitectureTests.cs)
