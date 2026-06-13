using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Configurations;

internal sealed class SiteAssetSummaryConfiguration : IEntityTypeConfiguration<SiteAssetSummary>
{
    public void Configure(EntityTypeBuilder<SiteAssetSummary> builder)
    {
        builder.ToTable("site_asset_summaries");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => SiteAssetSummaryId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.SiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .HasColumnName("site_id");

        builder.Property(a => a.CameraCount).IsRequired().HasColumnName("camera_count");
        builder.Property(a => a.DvrCount).IsRequired().HasColumnName("dvr_count");
        builder.Property(a => a.NvrCount).IsRequired().HasColumnName("nvr_count");
        builder.Property(a => a.HardDiskCount).IsRequired().HasColumnName("hard_disk_count");
        builder.Property(a => a.SwitchCount).IsRequired().HasColumnName("switch_count");
        builder.Property(a => a.RouterCount).IsRequired().HasColumnName("router_count");
        builder.Property(a => a.MonitorCount).IsRequired().HasColumnName("monitor_count");
        builder.Property(a => a.Brand).HasMaxLength(100).HasColumnName("brand");
        builder.Property(a => a.Model).HasMaxLength(100).HasColumnName("model");
        builder.Property(a => a.Remarks).HasMaxLength(1000).HasColumnName("remarks");
        builder.Property(a => a.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(a => a.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(a => a.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(a => a.UpdatedBy).HasColumnName("updated_by");
        builder.Property(a => a.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(a => a.SiteId).IsUnique().HasDatabaseName("ux_site_asset_summaries_site_id");

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_site_asset_summaries_counts_non_negative",
            "camera_count >= 0 AND dvr_count >= 0 AND nvr_count >= 0 AND hard_disk_count >= 0 " +
            "AND switch_count >= 0 AND router_count >= 0 AND monitor_count >= 0"));
    }
}
