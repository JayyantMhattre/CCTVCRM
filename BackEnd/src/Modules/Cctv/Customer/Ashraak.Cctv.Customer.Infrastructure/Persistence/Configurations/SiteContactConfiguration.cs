using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Configurations;

internal sealed class SiteContactConfiguration : IEntityTypeConfiguration<SiteContact>
{
    public void Configure(EntityTypeBuilder<SiteContact> builder)
    {
        builder.ToTable("site_contacts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => SiteContactId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.SiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .HasColumnName("site_id");

        builder.Property(c => c.ContactSlot).IsRequired().HasColumnName("contact_slot");
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(c => c.Designation).HasMaxLength(100).HasColumnName("designation");
        builder.Property(c => c.Phone).HasMaxLength(32).IsRequired().HasColumnName("phone");
        builder.Property(c => c.Email).HasMaxLength(320).HasColumnName("email");
        builder.Property(c => c.IsPrimary).IsRequired().HasColumnName("is_primary");
        builder.Property(c => c.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(c => c.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(c => c.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");

        builder.HasIndex(c => new { c.SiteId, c.ContactSlot })
            .IsUnique()
            .HasDatabaseName("ux_site_contacts_site_id_contact_slot");

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_site_contacts_contact_slot",
            "contact_slot >= 1 AND contact_slot <= 3"));
    }
}
