using Ashraak.Files.Domain.Aggregates.FileRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Files.Infrastructure.Persistence.Configurations;

internal sealed class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.ToTable("file_records");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .HasConversion(id => id.Value, value => FileRecordId.From(value))
            .HasColumnName("id");

        builder.Property(f => f.FileName).HasMaxLength(500).IsRequired().HasColumnName("file_name");
        builder.Property(f => f.ContentType).HasMaxLength(200).IsRequired().HasColumnName("content_type");
        builder.Property(f => f.StoragePath).HasMaxLength(1000).IsRequired().HasColumnName("storage_path");
        builder.Property(f => f.Size).IsRequired().HasColumnName("size");
        builder.Property(f => f.TenantId).IsRequired().HasColumnName("tenant_id");
        builder.Property(f => f.UploadedBy).IsRequired().HasColumnName("uploaded_by");
        builder.Property(f => f.UploadedOnUtc).IsRequired().HasColumnName("uploaded_on_utc");
        builder.Property(f => f.DeletedOnUtc).HasColumnName("deleted_on_utc");

        builder.Ignore(f => f.DomainEvents);
    }
}
