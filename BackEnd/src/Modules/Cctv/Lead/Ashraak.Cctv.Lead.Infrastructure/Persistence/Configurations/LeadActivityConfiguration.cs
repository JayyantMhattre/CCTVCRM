using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadActivity;
using Ashraak.Cctv.Lead.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence.Configurations;

internal sealed class LeadActivityConfiguration : IEntityTypeConfiguration<LeadActivity>
{
    public void Configure(EntityTypeBuilder<LeadActivity> builder)
    {
        builder.ToTable("lead_activities");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => LeadActivityId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.LeadId)
            .HasConversion(id => id.Value, value => LeadId.From(value))
            .IsRequired()
            .HasColumnName("lead_id");
        builder.Property(a => a.ActivityType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("activity_type");
        builder.Property(a => a.FromStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasColumnName("from_status");
        builder.Property(a => a.ToStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasColumnName("to_status");
        builder.Property(a => a.Description).HasMaxLength(4000).IsRequired().HasColumnName("description");
        builder.Property(a => a.OccurredAtUtc).IsRequired().HasColumnName("occurred_at");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(a => a.LeadId).HasDatabaseName("ix_lead_activities_lead_id");
    }
}
