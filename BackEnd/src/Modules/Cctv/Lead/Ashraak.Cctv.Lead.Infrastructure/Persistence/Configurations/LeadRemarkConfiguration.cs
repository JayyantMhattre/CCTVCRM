using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadRemark;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence.Configurations;

internal sealed class LeadRemarkConfiguration : IEntityTypeConfiguration<LeadRemark>
{
    public void Configure(EntityTypeBuilder<LeadRemark> builder)
    {
        builder.ToTable("lead_remarks");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => LeadRemarkId.From(value))
            .HasColumnName("id");

        builder.Property(r => r.LeadId)
            .HasConversion(id => id.Value, value => LeadId.From(value))
            .IsRequired()
            .HasColumnName("lead_id");
        builder.Property(r => r.Remark).HasMaxLength(4000).IsRequired().HasColumnName("remark");
        builder.Property(r => r.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(r => r.CreatedBy).IsRequired().HasColumnName("created_by");

        builder.HasIndex(r => r.LeadId).HasDatabaseName("ix_lead_remarks_lead_id");
    }
}
