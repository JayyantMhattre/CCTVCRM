using System.Text.Json;
using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.ApiKeys.Infrastructure.Persistence.Configurations;

internal sealed class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("api_keys");

        builder.HasKey(k => k.Id);
        builder.Property(k => k.Id)
            .HasConversion(id => id.Value, value => ApiKeyId.From(value))
            .HasColumnName("id");

        builder.Property(k => k.TenantId).IsRequired().HasColumnName("tenant_id");
        builder.Property(k => k.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(k => k.Description).HasMaxLength(1000).IsRequired().HasColumnName("description");
        builder.Property(k => k.KeyPrefix).HasMaxLength(64).IsRequired().HasColumnName("key_prefix");
        builder.Property(k => k.HashedSecret).HasMaxLength(512).IsRequired().HasColumnName("hashed_secret");
        builder.Property(k => k.Environment).HasMaxLength(16).IsRequired().HasColumnName("environment");
        builder.Property(k => k.Scopes)
            .HasColumnName("scopes")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v),
                v => JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>());
        builder.Property(k => k.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(k => k.CreatedOnUtc).IsRequired().HasColumnName("created_on_utc");
        builder.Property(k => k.ExpiresOnUtc).HasColumnName("expires_on_utc");
        builder.Property(k => k.LastUsedOnUtc).HasColumnName("last_used_on_utc");
        builder.Property(k => k.RevokedOnUtc).HasColumnName("revoked_on_utc");
        builder.Property(k => k.Enabled).IsRequired().HasColumnName("enabled");
        builder.Property(k => k.RequestCount).IsRequired().HasColumnName("request_count");
        builder.Property(k => k.SuccessCount).IsRequired().HasColumnName("success_count");
        builder.Property(k => k.FailureCount).IsRequired().HasColumnName("failure_count");
        builder.Property(k => k.LastCorrelationId).HasMaxLength(128).HasColumnName("last_correlation_id");

        builder.HasIndex(k => new { k.TenantId, k.Name }).IsUnique();
        builder.HasIndex(k => k.KeyPrefix).IsUnique();

        builder.Ignore(k => k.DomainEvents);
    }
}
