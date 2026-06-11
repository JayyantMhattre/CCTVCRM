using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Configurations;

internal sealed class WebhookDeadLetterConfiguration : IEntityTypeConfiguration<WebhookDeadLetter>
{
    public void Configure(EntityTypeBuilder<WebhookDeadLetter> builder)
    {
        builder.ToTable("webhook_dead_letters");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasConversion(id => id.Value, value => WebhookDeadLetterId.From(value))
            .HasColumnName("id");

        builder.Property(d => d.DeliveryId).IsRequired().HasColumnName("delivery_id");
        builder.Property(d => d.SubscriptionId).IsRequired().HasColumnName("subscription_id");
        builder.Property(d => d.TenantId).IsRequired().HasColumnName("tenant_id");
        builder.Property(d => d.EventName).HasMaxLength(200).IsRequired().HasColumnName("event_name");
        builder.Property(d => d.Payload).IsRequired().HasColumnName("payload");
        builder.Property(d => d.FailureReason).HasMaxLength(500).HasColumnName("failure_reason");
        builder.Property(d => d.FailureCode).HasColumnName("failure_code");
        builder.Property(d => d.RetryCount).IsRequired().HasColumnName("retry_count");
        builder.Property(d => d.CorrelationId).HasMaxLength(100).HasColumnName("correlation_id");
        builder.Property(d => d.CreatedOnUtc).IsRequired().HasColumnName("created_on_utc");

        builder.HasIndex(d => new { d.TenantId, d.CreatedOnUtc });
        builder.HasIndex(d => d.DeliveryId);
        builder.HasIndex(d => d.CorrelationId);

        builder.Ignore(d => d.DomainEvents);
    }
}
