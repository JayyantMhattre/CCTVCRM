# Audit — Registration

Entry point: `BackEnd/src/Modules/Audit/Ashraak.Audit.Infrastructure/AuditModule.cs`

Host order: **Layer 3** (last — observes all other modules).

## MongoDB

```csharp
var connectionString = configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string required.");

var mongoClient = new MongoClient(connectionString);
var databaseName = /* parsed from connection string, default "ashraak_audit" */;

services.AddSingleton<IMongoClient>(mongoClient);
services.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase(databaseName));
```

**Not using** `MongoDataModule` from BuildingBlocks.

Indexes ensured synchronously at registration via `EnsureIndexes`.

## Write pipeline services

| Registration | Lifetime |
|--------------|----------|
| `IAuditWriteQueue` → `AuditWriteQueue` | Singleton |
| `IAuditService` → `AuditRepository` | Singleton |
| `AuditMongoWriterHostedService` | HostedService |

## EF interceptor (cross-module)

```csharp
services.AddSingleton<IInterceptor, AuditEntityChangeInterceptor>();
```

Auth, Tenant, Users DbContexts resolve all `IInterceptor` instances — no direct Audit reference in those projects.

## MediatR

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventAuditHandler).Assembly));
```

## Middleware (host)

In `Program.cs` — **not** in AuditModule:

```csharp
app.UseAuthorization();
app.UseAuditApiCallLogging();  // AuditApiCallExtensions
app.UseOutputCache();
```

## Endpoint registration

```csharp
routeBuilder.MapAuditEndpoints();  // → /audit-logs under /api/v1
```

## Configuration keys

| Key | Required | Purpose |
|-----|----------|---------|
| `ConnectionStrings:MongoDB` | **Yes** | MongoDB connection (throws if missing) |

Default local: `mongodb://localhost:27017/ashraak_audit`

Database name parsed from connection string URI path; falls back to `ashraak_audit`.

## Host health check

`Program.cs` registers MongoDB health check with `ready` tag using same `IMongoClient`.

## Authorization

Audit endpoints use host policy `AdminOnly` (`RequireRole("Admin")`).

## Project references

- `Ashraak.SharedKernel`, `Ashraak.SharedKernel.Contracts`
- MongoDB.Driver (direct)

No EF Core reference in Audit module.
