using System.Text.Json;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Configurations;

internal sealed class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.ToTable("webhook_subscriptions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => WebhookSubscriptionId.From(value))
            .HasColumnName("id");

        builder.Property(s => s.TenantId).IsRequired().HasColumnName("tenant_id");
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(s => s.EndpointUrl).HasMaxLength(2000).IsRequired().HasColumnName("endpoint_url");
        builder.Property(s => s.SecretProtected).HasMaxLength(2000).IsRequired().HasColumnName("secret_protected");
        builder.Property(s => s.Enabled).IsRequired().HasColumnName("enabled");
        builder.Property(s => s.SubscribedEventNames)
            .HasColumnName("subscribed_event_names")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v),
                v => JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>());
        builder.Property(s => s.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(s => s.CreatedOnUtc).IsRequired().HasColumnName("created_on_utc");
        builder.Property(s => s.UpdatedOnUtc).IsRequired().HasColumnName("updated_on_utc");

        builder.HasIndex(s => new { s.TenantId, s.Name }).IsUnique();

        builder.Ignore(s => s.DomainEvents);
    }
}
