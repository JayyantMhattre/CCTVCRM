using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisitAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Visit.ServiceVisit;

namespace Ashraak.Cctv.Service.Infrastructure.Persistence.Configurations;

internal sealed class ServiceVisitConfiguration : IEntityTypeConfiguration<VisitAggregate>
{
    public void Configure(EntityTypeBuilder<VisitAggregate> builder)
    {
        builder.ToTable("service_visits");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => ServiceVisitId.From(value))
            .HasColumnName("id");
        builder.Property(v => v.ServiceScheduleId)
            .HasConversion(id => id.Value, value => ServiceScheduleId.From(value))
            .HasColumnName("service_schedule_id");
        builder.Property(v => v.EngineerId).IsRequired().HasColumnName("engineer_id");
        builder.Property(v => v.StartedAtUtc).HasColumnName("started_at");
        builder.Property(v => v.CompletedAtUtc).HasColumnName("completed_at");
        builder.Property(v => v.VisitRemarks).HasMaxLength(4000).HasColumnName("visit_remarks");
        builder.Property(v => v.ReportStatus).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("report_status");
        builder.Property(v => v.IsCustomerVisible).IsRequired().HasColumnName("is_customer_visible");
        builder.Property(v => v.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(v => v.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(v => v.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(v => v.UpdatedBy).HasColumnName("updated_by");
        builder.Property(v => v.RowVersion).HasColumnName("row_version").IsConcurrencyToken();
        builder.HasIndex(v => v.ServiceScheduleId).IsUnique().HasDatabaseName("ux_service_visits_service_schedule_id");
        builder.HasMany(v => v.Photos).WithOne().HasForeignKey(p => p.ServiceVisitId);
        builder.HasMany(v => v.Approvals).WithOne().HasForeignKey(a => a.ServiceVisitId);
        builder.HasMany(v => v.Attachments).WithOne().HasForeignKey(a => a.ServiceVisitId);
        builder.HasOne(v => v.Location).WithOne().HasForeignKey<VisitLocation>(l => l.ServiceVisitId);
        builder.HasOne(v => v.Signature).WithOne().HasForeignKey<VisitSignature>(s => s.ServiceVisitId);
        builder.Navigation(v => v.Photos).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(v => v.Approvals).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(v => v.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(v => v.HasSelfie);
        builder.Ignore(v => v.HasBeforeDuringAfterPhoto);
        builder.Ignore(v => v.HasGps);
        builder.Ignore(v => v.HasSignature);
        builder.Ignore(v => v.HasMinimumRemarks);
        builder.Ignore(v => v.DomainEvents);
    }
}

internal sealed class VisitPhotoConfiguration : IEntityTypeConfiguration<VisitPhoto>
{
    public void Configure(EntityTypeBuilder<VisitPhoto> builder)
    {
        builder.ToTable("visit_photos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.ServiceVisitId)
            .HasConversion(id => id.Value, value => ServiceVisitId.From(value))
            .HasColumnName("service_visit_id");
        builder.Property(p => p.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(p => p.Category).HasConversion<string>().HasMaxLength(16).IsRequired().HasColumnName("category");
        builder.Property(p => p.Caption).HasMaxLength(500).HasColumnName("caption");
        builder.Property(p => p.CapturedAtUtc).IsRequired().HasColumnName("captured_at");
        builder.Property(p => p.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(p => p.CreatedBy).IsRequired().HasColumnName("created_by");
    }
}

internal sealed class VisitLocationConfiguration : IEntityTypeConfiguration<VisitLocation>
{
    public void Configure(EntityTypeBuilder<VisitLocation> builder)
    {
        builder.ToTable("visit_locations");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.ServiceVisitId)
            .HasConversion(id => id.Value, value => ServiceVisitId.From(value))
            .HasColumnName("service_visit_id");
        builder.Property(l => l.Latitude).HasPrecision(9, 6).IsRequired().HasColumnName("latitude");
        builder.Property(l => l.Longitude).HasPrecision(9, 6).IsRequired().HasColumnName("longitude");
        builder.Property(l => l.CapturedAtUtc).IsRequired().HasColumnName("captured_at");
        builder.Property(l => l.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(l => l.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.HasIndex(l => l.ServiceVisitId).IsUnique().HasDatabaseName("ux_visit_locations_service_visit_id");
    }
}

internal sealed class VisitSignatureConfiguration : IEntityTypeConfiguration<VisitSignature>
{
    public void Configure(EntityTypeBuilder<VisitSignature> builder)
    {
        builder.ToTable("visit_signatures");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.ServiceVisitId)
            .HasConversion(id => id.Value, value => ServiceVisitId.From(value))
            .HasColumnName("service_visit_id");
        builder.Property(s => s.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(s => s.SignedByName).HasMaxLength(200).IsRequired().HasColumnName("signed_by_name");
        builder.Property(s => s.CapturedAtUtc).IsRequired().HasColumnName("captured_at");
        builder.Property(s => s.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(s => s.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.HasIndex(s => s.ServiceVisitId).IsUnique().HasDatabaseName("ux_visit_signatures_service_visit_id");
    }
}

internal sealed class VisitApprovalConfiguration : IEntityTypeConfiguration<VisitApproval>
{
    public void Configure(EntityTypeBuilder<VisitApproval> builder)
    {
        builder.ToTable("visit_approvals");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.ServiceVisitId)
            .HasConversion(id => id.Value, value => ServiceVisitId.From(value))
            .HasColumnName("service_visit_id");
        builder.Property(a => a.Decision).HasConversion<string>().HasMaxLength(16).IsRequired().HasColumnName("decision");
        builder.Property(a => a.ReviewedBy).HasColumnName("reviewed_by");
        builder.Property(a => a.ReviewedAtUtc).HasColumnName("reviewed_at");
        builder.Property(a => a.ReviewRemarks).HasMaxLength(2000).HasColumnName("review_remarks");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");
    }
}

internal sealed class VisitAttachmentConfiguration : IEntityTypeConfiguration<VisitAttachment>
{
    public void Configure(EntityTypeBuilder<VisitAttachment> builder)
    {
        builder.ToTable("visit_attachments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.ServiceVisitId)
            .HasConversion(id => id.Value, value => ServiceVisitId.From(value))
            .HasColumnName("service_visit_id");
        builder.Property(a => a.FileId).IsRequired().HasColumnName("file_id");
        builder.Property(a => a.AttachmentType).HasConversion<string>().HasMaxLength(32).IsRequired().HasColumnName("attachment_type");
        builder.Property(a => a.Title).HasMaxLength(200).HasColumnName("title");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");
    }
}
