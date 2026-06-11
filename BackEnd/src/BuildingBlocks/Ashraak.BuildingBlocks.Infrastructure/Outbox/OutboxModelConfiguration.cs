using Ashraak.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Shared EF configuration for outbox tables.
/// </summary>
public static class OutboxModelConfiguration
{
    public static void ConfigureOutboxMessages(ModelBuilder modelBuilder, string tableName = "outbox_messages")
    {
        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable(tableName);
            b.HasKey(m => m.Id);
            b.Property(m => m.Type).HasMaxLength(500).IsRequired();
            b.Property(m => m.Content).IsRequired();
            b.Property(m => m.Error).HasMaxLength(2000);
            b.HasIndex(m => new { m.ProcessedOnUtc, m.CreatedOnUtc });
        });
    }
}
