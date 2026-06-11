# Shared Kernel — Operations

Operational notes for the shared foundation layer.

## NuGet dependencies

Central versions in `BackEnd/Directory.Packages.props`:

| Package | Version | Used by |
|---------|---------|---------|
| MediatR | 12.4.1 | SharedKernel (IDomainEvent extends INotification) |

SharedKernel.Contracts references SharedKernel only — no additional packages.

## Build and compatibility

- **Target:** `net10.0` via `BackEnd/Directory.Build.props`
- **Nullable:** enabled
- **Warnings:** treated as errors

Any breaking change to SharedKernel or Contracts requires rebuilding all module projects. Run:

```powershell
dotnet build BackEnd/Ashraak.slnx
```

Architecture tests:

```powershell
dotnet test BackEnd/tests/Ashraak.Architecture.Tests/
```

## Deployment impact

Shared Kernel is linked into every module assembly — there is no separate deployable. Changes ship with the host (`Ashraak.Api`) on every release.

## Outbox tables (scaffold)

Module DbContexts declare outbox DbSets but do not populate them today:

| Schema | Table | DbContext |
|--------|-------|-----------|
| `auth` | `outbox_messages` | `AuthDbContext` |
| `tenant` | `outbox_messages` | `TenantDbContext` |
| `users` | `outbox_messages` | `UsersDbContext` |

No migrations folder is present in the repo at time of writing. Before enabling outbox writes, generate and apply EF migrations per module.

## Event delivery guarantees

Current in-process MediatR delivery:

- **Synchronous** within the request scope after save
- **No retry** on handler failure
- **No persistence** if the process crashes after save but before publish
- **Lost on restart** — no outbox replay

Plan for production: wire `BaseDbContext` + `OutboxProcessorBase` before relying on cross-module consistency.

## Monitoring

Contract events captured by Audit module's `DomainEventAuditHandler` appear in MongoDB `audit_entries` collection when published. Check Audit module operations for MongoDB connectivity.

## Troubleshooting

| Symptom | Likely cause |
|---------|--------------|
| Handler never runs | Event type mismatch (domain vs contract event) |
| `ITenantService` null resolution | Tenant module not registered before consumer |
| Outbox table empty | DbContexts don't inherit `BaseDbContext` |
| Audit missing events | Event not published via MediatR |

## Related configuration

Shared Kernel types do not read `appsettings.json`. See [Host operations](../host/operations.md) for connection strings and environment variables that affect runtime abstractions (JWT claims → `ICurrentUser`, `ITenantContext`).
