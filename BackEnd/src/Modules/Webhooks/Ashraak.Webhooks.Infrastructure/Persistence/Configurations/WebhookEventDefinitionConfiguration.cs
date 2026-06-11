using Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Configurations;

internal sealed class WebhookEventDefinitionConfiguration : IEntityTypeConfiguration<WebhookEventDefinition>
{
    public void Configure(EntityTypeBuilder<WebhookEventDefinition> builder)
    {
        builder.ToTable("webhook_event_definitions");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EventName).HasMaxLength(200).IsRequired().HasColumnName("event_name");
        builder.Property(e => e.Version).HasMaxLength(20).IsRequired().HasColumnName("version");
        builder.Property(e => e.Description).HasMaxLength(1000).IsRequired().HasColumnName("description");
        builder.Property(e => e.Enabled).IsRequired().HasColumnName("enabled");

        builder.HasIndex(e => e.EventName).IsUnique();
    }
}
