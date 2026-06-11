# Database Abstraction Layer

> **Canonical:** [docs/modules/building-blocks/](../../docs/modules/building-blocks/README.md)  
> **Location**: `src/BuildingBlocks/`  
> **Projects**: `Data.Abstractions` · `Data.Sql` · `Data.Mongo`  
> **Status:** Optional libraries — feature modules use EF/Mongo directly today.

---

## 1 · Overview

The data layer provides a **provider-agnostic, Clean-Architecture-compliant** repository pattern that supports **SQL Server**, **PostgreSQL**, **MySQL**, **Oracle**, and **MongoDB** through a unified interface.  Application-layer handlers depend only on the abstractions project; the concrete SQL or MongoDB implementation is resolved at startup via DI configuration.

```
┌─────────────────────────────────────────────────────────────────────┐
│  Application Layer (Command / Query Handlers)                        │
│  ► IDataRepository<T>  IReadRepository<T>  IDataUnitOfWork          │
└──────────────────────────┬──────────────────────────────────────────┘
                           │ depends on
┌──────────────────────────▼──────────────────────────────────────────┐
│  BuildingBlocks.Data.Abstractions                                    │
│  ► Interfaces  ► Common types  ► Configuration                       │
└──────────┬────────────────────────────────────────┬─────────────────┘
           │ implements                              │ implements
┌──────────▼──────────────┐           ┌─────────────▼─────────────────┐
│  BuildingBlocks.Data.Sql│           │  BuildingBlocks.Data.Mongo     │
│  GenericSqlRepository   │           │  MongoDataRepository           │
│  SqlDataUnitOfWork      │           │  MongoDataUnitOfWork           │
│  4 Connection Factories │           │  MongoIndexService             │
└─────────────────────────┘           └───────────────────────────────┘
```

### Relationship to the existing DDD repositories

| Layer | Base class | Used for | Persists via |
|---|---|---|---|
| DDD repositories | `BaseRepository<TAggregate, TId>` | Aggregate roots with strongly-typed IDs, domain events | Module's own `DbContext` |
| **Data layer (new)** | `BaseDataEntity` | General entities, read models, cross-cutting collections | `IDataRepository<T>` + chosen provider |

Both layers co-exist.  The data layer is *not* a replacement.

---

## 2 · Project Structure

```
src/BuildingBlocks/
├── Ashraak.BuildingBlocks.Data.Abstractions/
│   ├── Common/
│   │   ├── BaseDataEntity.cs          ← Guid Id + audit fields
│   │   ├── TenantScopedEntity.cs      ← adds TenantId
│   │   ├── ITenantScoped.cs           ← marker interface
│   │   ├── PagedResult.cs             ← pagination wrapper
│   │   ├── SortDescriptor.cs          ← dynamic sort
│   │   └── QueryOptions.cs            ← EF Core includes / tracking
│   ├── Interfaces/
│   │   ├── IReadRepository.cs
│   │   ├── IWriteRepository.cs
│   │   ├── IAdvancedRepository.cs     ← raw SQL, stored procs
│   │   ├── IDataRepository.cs         ← combines all three
│   │   ├── IDataUnitOfWork.cs         ← extends IUnitOfWork
│   │   └── IDbConnectionFactory.cs    ← Dapper ADO.NET factory
│   └── Configuration/
│       ├── DatabaseOptions.cs         ← options POCO
│       └── DatabaseProviderType.cs    ← enum: SqlServer | PostgreSql | MySql | Oracle | MongoDB
│
├── Ashraak.BuildingBlocks.Data.Sql/
│   ├── GenericSqlRepository.cs        ← EF Core + Dapper (all operations)
│   ├── SqlDataUnitOfWork.cs           ← IDbContextTransaction wrapper
│   ├── Providers/
│   │   ├── PostgreSqlConnectionFactory.cs
│   │   ├── SqlServerConnectionFactory.cs
│   │   ├── MySqlConnectionFactory.cs
│   │   └── OracleConnectionFactory.cs
│   └── SqlDataModule.cs               ← AddSqlDataLayer<TContext>() DI extension
│
└── Ashraak.BuildingBlocks.Data.Mongo/
    ├── MongoDataRepository.cs         ← MongoDB.Driver (all operations)
    ├── MongoDataUnitOfWork.cs         ← IClientSession wrapper
    ├── MongoIndexService.cs           ← index creation helpers
    └── MongoDataModule.cs             ← AddMongoDataLayer() + AddMongoRepository<T>()
```

---

## 3 · Base Entities

```csharp
// Inherit this for any entity in the SQL/Mongo data layer.
public abstract class BaseDataEntity : Entity<Guid>   // SharedKernel.Entity<Guid>
{
    public Guid        Id            { get; }          // inherited
    public DateTime    CreatedAtUtc  { get; set; }
    public DateTime?   UpdatedAtUtc  { get; set; }
    public bool        IsDeleted     { get; set; }
    public DateTime?   DeletedAtUtc  { get; set; }
}

// Adds automatic TenantId stamping + filtering.
public abstract class TenantScopedEntity : BaseDataEntity, ITenantScoped
{
    public Guid TenantId { get; set; }
}
```

---

## 4 · Core Interfaces

```csharp
// Full surface — use in command handlers that read AND write.
public interface IDataRepository<T> : IReadRepository<T>, IWriteRepository<T>, IAdvancedRepository
    where T : BaseDataEntity { }

// Read-only surface — use in query handlers.
public interface IReadRepository<T> where T : BaseDataEntity
{
    Task<T?>                 GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>>   GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>>   GetByFilterAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
    Task<IReadOnlyList<T>>   GetByFilterAsync(Expression<Func<T, bool>> filter, QueryOptions<T> options, CancellationToken ct = default);
    Task<PagedResult<T>>     GetPagedAsync(int page, int size, Expression<Func<T,bool>>? filter = null,
                                           IEnumerable<SortDescriptor<T>>? sort = null, CancellationToken ct = default);
    Task<bool>               ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
    Task<long>               CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
    IQueryable<T>            Query(bool tracking = false);  // EF Core only
}

// Write surface.
public interface IWriteRepository<T> where T : BaseDataEntity
{
    Task         InsertAsync(T entity, CancellationToken ct = default);
    Task         InsertBulkAsync(IEnumerable<T> entities, CancellationToken ct = default);
    Task         UpdateAsync(T entity, CancellationToken ct = default);
    Task<int>    UpdateByFilterAsync(Expression<Func<T,bool>> filter, Action<T> updater, CancellationToken ct = default);
    Task         DeleteAsync(T entity, CancellationToken ct = default);
    Task<int>    DeleteByFilterAsync(Expression<Func<T, bool>> filter, CancellationToken ct = default);
    Task<bool>   SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

// Advanced — SQL providers only.
public interface IAdvancedRepository
{
    Task<IReadOnlyList<TResult>> ExecuteRawSqlAsync<TResult>(string sql, object? parameters = null, CancellationToken ct = default);
    Task<int>                    ExecuteStoredProcedureAsync(string name, object? parameters = null, CancellationToken ct = default);
}

// Transaction management.
public interface IDataUnitOfWork : IUnitOfWork          // from SharedKernel
{
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
    // SaveChangesAsync(ct) inherited from IUnitOfWork
}
```

---

## 5 · Configuration

**`appsettings.json`** (or environment variables):

```json
{
  "Database": {
    "Provider": "PostgreSql",
    "ConnectionString": "Host=localhost;Port=5432;Database=ashraak;Username=app;Password=secret",
    "DatabaseName": "",
    "CommandTimeout": 30,
    "EnableDetailedErrors": false,
    "EnableSensitiveDataLogging": false
  }
}
```

| `Provider` value | Registers | Dapper driver |
|---|---|---|
| `PostgreSql` | `Npgsql.EntityFrameworkCore.PostgreSQL` | `Npgsql` |
| `SqlServer`  | `Microsoft.EntityFrameworkCore.SqlServer` | `Microsoft.Data.SqlClient` |
| `MySql`      | `Pomelo.EntityFrameworkCore.MySql` | `MySqlConnector` |
| `Oracle`     | `Oracle.EntityFrameworkCore` | `Oracle.ManagedDataAccess.Core` |
| `MongoDB`    | `MongoDB.Driver` (no EF Core) | n/a |

---

## 6 · DI Registration

### SQL provider (per module)

```csharp
// In BillingModule / ServiceCollectionExtensions:
public static IServiceCollection AddBillingModule(
    this IServiceCollection services, IConfiguration configuration)
{
    // 1. Register EF Core + Dapper connection factory + IDataUnitOfWork
    //    bound to BillingDbContext.
    services.AddSqlDataLayer<BillingDbContext>(configuration);

    // 2. Register per-entity repositories.
    services.AddScoped<IDataRepository<Invoice>>(sp =>
        new GenericSqlRepository<Invoice>(
            sp.GetRequiredService<BillingDbContext>(),
            sp.GetRequiredService<IDbConnectionFactory>(),
            sp.GetService<ITenantContext>()));

    return services;
}
```

### MongoDB provider

```csharp
// Once in Program.cs or a shared infrastructure module:
services.AddMongoDataLayer(configuration);

// Per entity in the module that owns the collection:
services.AddMongoRepository<AuditLog>();          // collection name defaults to "auditLogs"
services.AddMongoRepository<AuditLog>("audit");   // explicit collection name
```

### Switching providers via environment variable

```bash
# Switch the entire app to SQL Server without changing code:
Database__Provider=SqlServer
Database__ConnectionString="Server=sql.prod;Database=ashraak;..."
```

---

## 7 · Example Usage in a Service

```csharp
// ─── Command handler (write) ──────────────────────────────────────────

public sealed class CreateInvoiceHandler(
    IDataRepository<Invoice> invoices,
    IDataUnitOfWork uow)
{
    public async Task<Guid> Handle(CreateInvoiceCommand cmd, CancellationToken ct)
    {
        var invoice = new Invoice
        {
            Number = cmd.Number,
            Amount = cmd.Amount
            // TenantId is stamped automatically from ITenantContext.
        };

        await invoices.InsertAsync(invoice, ct);
        await uow.SaveChangesAsync(ct);
        return invoice.Id;
    }
}

// ─── Query handler (read) ─────────────────────────────────────────────

public sealed class GetInvoicesHandler(IReadRepository<Invoice> invoices)
{
    public async Task<PagedResult<Invoice>> Handle(GetInvoicesQuery q, CancellationToken ct)
        => await invoices.GetPagedAsync(
            pageNumber: q.Page,
            pageSize: q.PageSize,
            filter: x => x.Amount > q.MinAmount,
            sortDescriptors: [new() { KeySelector = x => x.CreatedAtUtc, Descending = true }],
            cancellationToken: ct);
}

// ─── Dapper raw SQL (high-performance report) ─────────────────────────

public sealed class InvoiceSummaryHandler(IDataRepository<Invoice> repo)
{
    public async Task<IReadOnlyList<InvoiceSummaryDto>> Handle(
        InvoiceSummaryQuery q, CancellationToken ct)
        => await repo.ExecuteRawSqlAsync<InvoiceSummaryDto>(
            """
            SELECT date_trunc('month', created_at_utc) AS month,
                   SUM(amount) AS total
            FROM   billing.invoices
            WHERE  tenant_id = @tenantId
              AND  is_deleted = false
            GROUP  BY 1
            ORDER  BY 1 DESC
            LIMIT  12
            """,
            parameters: new { tenantId = q.TenantId },
            cancellationToken: ct);
}

// ─── Transaction (two-step create + publish) ──────────────────────────

public sealed class CheckoutHandler(
    IDataRepository<Order> orders,
    IDataRepository<Inventory> inventory,
    IDataUnitOfWork uow)
{
    public async Task Handle(CheckoutCommand cmd, CancellationToken ct)
    {
        await uow.BeginTransactionAsync(ct);
        try
        {
            await orders.InsertAsync(new Order(cmd), ct);
            await inventory.UpdateByFilterAsync(
                x => x.ProductId == cmd.ProductId,
                x => x.Quantity -= cmd.Quantity,
                ct);

            await uow.SaveChangesAsync(ct);
            await uow.CommitTransactionAsync(ct);
        }
        catch
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
```

---

## 8 · MongoDB Index Setup

Create indexes at application startup to avoid full-collection scans:

```csharp
// In a hosted service or module startup:
public class AuditIndexBootstrap(MongoIndexService indexes) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        // Compound index on the most-queried combination.
        await indexes.CreateCompoundIndexAsync<AuditLog>(
            [(x => x.TenantId, false), (x => x.CreatedAtUtc, true)],
            cancellationToken: ct);

        // Full-text search on the action description field.
        await indexes.CreateTextIndexAsync<AuditLog>(x => x.Action, cancellationToken: ct);

        // TTL: auto-delete logs older than 90 days.
        await indexes.CreateTtlIndexAsync<AuditLog>(
            x => x.CreatedAtUtc,
            expireAfter: TimeSpan.FromDays(90),
            cancellationToken: ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
```

---

## 9 · Multi-Tenancy

Every query that hits a `TenantScopedEntity` collection/table has **automatic tenant isolation**:

| Layer | Mechanism |
|---|---|
| SQL (GenericSqlRepository) | `WHERE tenant_id = @currentTenant` appended to every `IQueryable` |
| MongoDB (MongoDataRepository) | `Filter.Eq("TenantId", currentTenant)` added to every `FilterDefinition` |
| Insert / Bulk insert | `TenantId` stamped from `ITenantContext.TenantId` when not already set |
| Background jobs | Pass `ITenantContext = null` — filter is skipped; job sees all tenants |

---

## 10 · Adding the New Projects to the Solution

```powershell
cd D:\AntiGravity\Ashraak\BackEnd

dotnet sln add src/BuildingBlocks/Ashraak.BuildingBlocks.Data.Abstractions/Ashraak.BuildingBlocks.Data.Abstractions.csproj
dotnet sln add src/BuildingBlocks/Ashraak.BuildingBlocks.Data.Sql/Ashraak.BuildingBlocks.Data.Sql.csproj
dotnet sln add src/BuildingBlocks/Ashraak.BuildingBlocks.Data.Mongo/Ashraak.BuildingBlocks.Data.Mongo.csproj
```

Verify the build compiles:

```powershell
dotnet build src/BuildingBlocks/Ashraak.BuildingBlocks.Data.Abstractions
dotnet build src/BuildingBlocks/Ashraak.BuildingBlocks.Data.Sql
dotnet build src/BuildingBlocks/Ashraak.BuildingBlocks.Data.Mongo
```

---

## 11 · Design Decisions

| Decision | Rationale |
|---|---|
| `BaseDataEntity` extends `Entity<Guid>` | Full compatibility with SharedKernel equality/hash semantics |
| Soft-delete is always applied | Prevents accidental data loss; hard-delete available when needed |
| `UpdateByFilterAsync` uses load-then-save | Provider-agnostic; for high-throughput bulk updates use `ExecuteRawSqlAsync` |
| `IQueryable.Query()` throws on MongoDB | Signals the contract honestly; callers choose the right query method |
| `MongoDataUnitOfWork.SaveChangesAsync` is a no-op | MongoDB writes are immediate; no change tracker exists |
| Guid stored as string in MongoDB | Human-readable in Compass; consistent with REST API representations |
| Connection factories are Scoped | PgBouncer / connection pool manages physical connections; factory resolves quickly |
| `EnableSensitiveDataLogging` is configurable | Dev-only flag; never enabled in production configs |
