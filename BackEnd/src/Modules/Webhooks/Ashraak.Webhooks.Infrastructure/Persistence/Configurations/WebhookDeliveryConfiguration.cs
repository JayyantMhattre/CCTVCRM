using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Configurations;

internal sealed class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
    {
        builder.ToTable("webhook_deliveries");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasConversion(id => id.Value, value => WebhookDeliveryId.From(value))
            .HasColumnName("id");

        builder.Property(d => d.SubscriptionId).IsRequired().HasColumnName("subscription_id");
        builder.Property(d => d.TenantId).IsRequired().HasColumnName("tenant_id");
        builder.Property(d => d.EventName).HasMaxLength(200).IsRequired().HasColumnName("event_name");
        builder.Property(d => d.EventVersion).HasMaxLength(20).IsRequired().HasColumnName("event_version");
        builder.Property(d => d.CorrelationId).HasMaxLength(100).HasColumnName("correlation_id");
        builder.Property(d => d.Payload).IsRequired().HasColumnName("payload");
        builder.Property(d => d.AttemptNumber).IsRequired().HasColumnName("attempt_number");
        builder.Property(d => d.RetryCount).IsRequired().HasColumnName("retry_count");
        builder.Property(d => d.LastFailureReason).HasMaxLength(500).HasColumnName("last_failure_reason");
        builder.Property(d => d.LastFailureCode).HasColumnName("last_failure_code");
        builder.Property(d => d.NextRetryOnUtc).HasColumnName("next_retry_on_utc");
        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(d => d.ResponseCode).HasColumnName("response_code");
        builder.Property(d => d.ResponseBody).HasMaxLength(4000).HasColumnName("response_body");
        builder.Property(d => d.StartedOnUtc).IsRequired().HasColumnName("started_on_utc");
        builder.Property(d => d.CompletedOnUtc).HasColumnName("completed_on_utc");

        builder.HasIndex(d => new { d.TenantId, d.StartedOnUtc });
        builder.HasIndex(d => new { d.SubscriptionId, d.StartedOnUtc });
        builder.HasIndex(d => d.CorrelationId);
        builder.HasIndex(d => new { d.Status, d.NextRetryOnUtc });

        builder.Ignore(d => d.DomainEvents);
    }
}
