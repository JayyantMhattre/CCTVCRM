using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.ApiKeys.Infrastructure.Persistence;
using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.ApiKeys.Infrastructure.Outbox;

internal sealed class ApiKeysOutboxWriter(ApiKeysDbContext dbContext) : IOutboxWriter
{
    public void Enqueue(IDomainEvent domainEvent) =>
        OutboxDomainEventSerializer.Enqueue(dbContext.OutboxMessages, domainEvent);
}
