using Ashraak.SharedKernel.Domain.Events;
using Ashraak.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// EF-backed <see cref="IOutboxWriter"/> for a module DbContext with an <see cref="OutboxMessage"/> set.
/// </summary>
internal sealed class EfOutboxWriter<TDbContext>(TDbContext dbContext) : IOutboxWriter
    where TDbContext : DbContext
{
    private DbSet<OutboxMessage> OutboxMessages => dbContext.Set<OutboxMessage>();

    public void Enqueue(IDomainEvent domainEvent) =>
        OutboxDomainEventSerializer.Enqueue(OutboxMessages, domainEvent);
}
