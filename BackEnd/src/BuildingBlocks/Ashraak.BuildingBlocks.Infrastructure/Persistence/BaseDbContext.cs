using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// Abstract base for every module's DbContext.
/// Serialises domain events from aggregate roots into the outbox table before commit.
/// Each module's DbContext inherits this and adds its own DbSets + OnModelCreating config.
/// </summary>
public abstract class BaseDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SerializeDomainEventsToOutbox();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Type).HasMaxLength(500).IsRequired();
            b.Property(m => m.Content).IsRequired();
        });
    }

    private void SerializeDomainEventsToOutbox() =>
        OutboxDomainEventSerializer.SerializeTrackedDomainEvents(this, OutboxMessages);
}
