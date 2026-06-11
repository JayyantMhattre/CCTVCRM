using Ashraak.SharedKernel.Domain.Events;
using Ashraak.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Serialises aggregate domain events and explicit outbox messages into <see cref="OutboxMessage"/> rows.
/// </summary>
public static class OutboxDomainEventSerializer
{
    /// <summary>
    /// Moves domain events from tracked aggregates into the outbox set.
    /// </summary>
    public static void SerializeTrackedDomainEvents(DbContext dbContext, DbSet<OutboxMessage> outboxMessages)
    {
        var domainEvents = dbContext.ChangeTracker
            .Entries()
            .Select(e => e.Entity)
            .OfType<IHasDomainEvents>()
            .SelectMany(aggregate =>
            {
                var events = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();
                return events;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
            Enqueue(outboxMessages, domainEvent);
    }

    /// <summary>
    /// Adds a single event to the outbox (contract or domain events).
    /// </summary>
    public static void Enqueue(DbSet<OutboxMessage> outboxMessages, IDomainEvent domainEvent)
    {
        var message = OutboxMessage.Create(
            type: domainEvent.GetType().AssemblyQualifiedName!,
            content: JsonSerializer.Serialize(domainEvent, domainEvent.GetType()));

        outboxMessages.Add(message);
    }
}
