using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Enums;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;
using SiteId = Ashraak.Cctv.Customer.Domain.Aggregates.Site.SiteId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Configurations;

internal sealed class SiteConfiguration : IEntityTypeConfiguration<SiteAggregate>
{
    public void Configure(EntityTypeBuilder<SiteAggregate> builder)
    {
        builder.ToTable("sites");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .HasColumnName("id");

        builder.Property(s => s.CustomerId)
            .HasConversion(id => id.Value, value => CustomerId.From(value))
            .HasColumnName("customer_id");

        builder.Property(s => s.SiteNumber).HasMaxLength(32).IsRequired().HasColumnName("site_number");
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(s => s.Address).HasMaxLength(500).IsRequired().HasColumnName("address");
        builder.Property(s => s.City).HasMaxLength(100).IsRequired().HasColumnName("city");
        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(s => s.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(s => s.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(s => s.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");
        builder.Property(s => s.IsDeleted).IsRequired().HasColumnName("is_deleted");
        builder.Property(s => s.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(s => s.SiteNumber).IsUnique().HasDatabaseName("ux_sites_site_number");
        builder.HasIndex(s => s.CustomerId).HasDatabaseName("ix_sites_customer_id");
        builder.HasIndex(s => s.Status).HasDatabaseName("ix_sites_status");
        builder.HasIndex(s => s.CreatedAtUtc).HasDatabaseName("ix_sites_created_at");

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasMany(s => s.Contacts)
            .WithOne()
            .HasForeignKey(c => c.SiteId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(s => s.Contacts).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.Documents)
            .WithOne()
            .HasForeignKey(d => d.SiteId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(s => s.Documents).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(s => s.AssetSummary)
            .WithOne()
            .HasForeignKey<Domain.Aggregates.Site.SiteAssetSummary>(a => a.SiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.DomainEvents);
    }
}
