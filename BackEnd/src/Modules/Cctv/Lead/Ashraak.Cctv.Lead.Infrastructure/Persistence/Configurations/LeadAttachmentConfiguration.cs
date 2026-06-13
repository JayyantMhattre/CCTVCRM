using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence.Configurations;

internal sealed class LeadAttachmentConfiguration : IEntityTypeConfiguration<LeadAttachment>
{
    public void Configure(EntityTypeBuilder<LeadAttachment> builder)
    {
        builder.ToTable("lead_attachments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => LeadAttachmentId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.LeadId)
            .HasConversion(id => id.Value, value => LeadId.From(value))
            .IsRequired()
            .HasColumnName("lead_id");
        builder.Property(a => a.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(a => a.Title).HasMaxLength(500).IsRequired().HasColumnName("title");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(a => a.IsDeleted).IsRequired().HasColumnName("is_deleted");

        builder.HasIndex(a => a.LeadId).HasDatabaseName("ix_lead_attachments_lead_id");
    }
}
