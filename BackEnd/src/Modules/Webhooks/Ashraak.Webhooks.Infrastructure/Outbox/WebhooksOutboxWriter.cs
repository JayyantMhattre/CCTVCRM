using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Domain.Events;
using Ashraak.Webhooks.Infrastructure.Persistence;

namespace Ashraak.Webhooks.Infrastructure.Outbox;

internal sealed class WebhooksOutboxWriter(WebhooksDbContext dbContext) : IOutboxWriter
{
    public void Enqueue(IDomainEvent domainEvent) =>
        OutboxDomainEventSerializer.Enqueue(dbContext.OutboxMessages, domainEvent);
}
