# Building Blocks — API Reference

Paths relative to `BackEnd/src/BuildingBlocks/`.

## Application

| Type | Path | Signature / role |
|------|------|------------------|
| `ICommand` | `Ashraak.BuildingBlocks.Application/Commands/ICommand.cs` | `IRequest<Result>` |
| `ICommand<T>` | same | `IRequest<Result<T>>` |
| `ICommandHandler<TCommand>` | `Commands/ICommandHandler.cs` | `IRequestHandler<TCommand, Result>` |
| `IQuery<TResponse>` | `Queries/IQuery.cs` | `IRequest<Result<TResponse>>` |
| `IQueryHandler<TQuery,TResponse>` | `Queries/IQueryHandler.cs` | Handler alias |
| `ValidationBehavior<TRequest,TResponse>` | `Behaviors/ValidationBehavior.cs` | Runs FluentValidation validators |
| `LoggingBehavior<TRequest,TResponse>` | `Behaviors/LoggingBehavior.cs` | Logs request type and duration |
| `PerformanceBehavior<TRequest,TResponse>` | `Behaviors/PerformanceBehavior.cs` | Warns if > 500 ms |

## Infrastructure

| Type | Path | Role |
|------|------|------|
| `BaseDbContext` | `Ashraak.BuildingBlocks.Infrastructure/Persistence/BaseDbContext.cs` | Outbox serialization on save |
| `BaseRepository<TAggregate,TId>` | `Persistence/BaseRepository.cs` | Generic aggregate CRUD |
| `OutboxProcessorBase` | `Outbox/OutboxProcessorBase.cs` | Abstract Quartz outbox dispatcher |

### BaseDbContext key methods

```csharp
public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
protected void SerializeDomainEventsToOutbox();
protected override void OnModelCreating(ModelBuilder modelBuilder);
```

## EventBus

| Type | Path | Role |
|------|------|------|
| `IIntegrationEvent` | `Ashraak.BuildingBlocks.EventBus/IIntegrationEvent.cs` | Broker event marker |
| `IntegrationEvent` | `IntegrationEvent.cs` | Base record with `EventId`, `OccurredOnUtc` |
| `IEventBus` | `IEventBus.cs` | `Task PublishAsync<TEvent>(TEvent, CancellationToken)` |
| `InProcessEventBus` | `InProcessEventBus.cs` | Log-only stub implementation |

## Data.Abstractions

| Type | Path | Role |
|------|------|------|
| `BaseDataEntity` | `Data.Abstractions/Entities/BaseDataEntity.cs` | Id + audit columns |
| `TenantScopedEntity` | `Entities/TenantScopedEntity.cs` | Tenant-scoped base |
| `IDataRepository<T>` | `Repositories/IDataRepository.cs` | CRUD + query |
| `IReadRepository<T>` | `Repositories/IReadRepository.cs` | Read-only |
| `IWriteRepository<T>` | `Repositories/IWriteRepository.cs` | Write-only |
| `IDataUnitOfWork` | `UnitOfWork/IDataUnitOfWork.cs` | Transactional UoW |
| `PagedResult<T>` | `Query/PagedResult.cs` | Pagination result |
| `QueryOptions<T>` | `Query/QueryOptions.cs` | Filter/sort/page |
| `DatabaseOptions` | `Configuration/DatabaseOptions.cs` | `Database` config section |
| `DatabaseProviderType` | `Configuration/DatabaseProviderType.cs` | SqlServer, PostgreSql, MySql, Oracle |

## Data.Sql

| Type | Path | Role |
|------|------|------|
| `SqlDataModule` | `Data.Sql/SqlDataModule.cs` | `AddSqlDataLayer<TContext>(configuration)` |
| `GenericSqlRepository<T>` | `Repositories/GenericSqlRepository.cs` | EF-based generic repo |
| `SqlDataUnitOfWork` | `UnitOfWork/SqlDataUnitOfWork.cs` | Transaction management |
| `IDbConnectionFactory` | `Connections/IDbConnectionFactory.cs` | Dapper connection factory |

## Data.Mongo

| Type | Path | Role |
|------|------|------|
| `MongoDataModule` | `Data.Mongo/MongoDataModule.cs` | `AddMongoDataLayer`, `AddMongoRepository<T>()` |
| `MongoDataRepository<T>` | `Repositories/MongoDataRepository.cs` | Generic Mongo CRUD |
| `MongoDataUnitOfWork` | `UnitOfWork/MongoDataUnitOfWork.cs` | Mongo session UoW |
| `MongoIndexService` | `Indexing/MongoIndexService.cs` | Compound/text/TTL indexes |

## Package versions

From `BackEnd/Directory.Packages.props`:

| Package | Version |
|---------|---------|
| Microsoft.EntityFrameworkCore | 9.0.4 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 |
| Quartz | 3.14.0 |
| MediatR | 12.4.1 |
| Dapper | (central version) |
