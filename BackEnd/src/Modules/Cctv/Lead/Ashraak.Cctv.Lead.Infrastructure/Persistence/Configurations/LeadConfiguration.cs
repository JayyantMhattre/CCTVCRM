using Ashraak.Cctv.Lead.Domain.Enums;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence.Configurations;

internal sealed class LeadConfiguration : IEntityTypeConfiguration<LeadAggregate>
{
    public void Configure(EntityTypeBuilder<LeadAggregate> builder)
    {
        builder.ToTable("leads");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, value => LeadId.From(value))
            .HasColumnName("id");

        builder.Property(l => l.LeadNumber).HasMaxLength(32).IsRequired().HasColumnName("lead_number");
        builder.Property(l => l.Source)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("source");
        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(l => l.ContactName).HasMaxLength(200).IsRequired().HasColumnName("contact_name");
        builder.Property(l => l.OrganizationName).HasMaxLength(200).HasColumnName("organization_name");
        builder.Property(l => l.Email).HasMaxLength(320).IsRequired().HasColumnName("email");
        builder.Property(l => l.Phone).HasMaxLength(32).IsRequired().HasColumnName("phone");
        builder.Property(l => l.City).HasMaxLength(100).IsRequired().HasColumnName("city");
        builder.Property(l => l.Address).HasMaxLength(500).HasColumnName("address");
        builder.Property(l => l.RequirementSummary).HasMaxLength(4000).HasColumnName("requirement_summary");
        builder.Property(l => l.OwnerUserId).HasColumnName("owner_user_id");
        builder.Property(l => l.ConvertedCustomerId).HasColumnName("converted_customer_id");
        builder.Property(l => l.ConvertedSiteId).HasColumnName("converted_site_id");
        builder.Property(l => l.ConvertedContractId).HasColumnName("converted_contract_id");
        builder.Property(l => l.ConvertedAtUtc).HasColumnName("converted_at");
        builder.Property(l => l.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(l => l.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(l => l.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(l => l.UpdatedBy).HasColumnName("updated_by");
        builder.Property(l => l.IsDeleted).IsRequired().HasColumnName("is_deleted");
        builder.Property(l => l.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(l => l.LeadNumber).IsUnique().HasDatabaseName("ux_leads_lead_number");
        builder.HasIndex(l => l.Status).HasDatabaseName("ix_leads_status");
        builder.HasIndex(l => l.CreatedAtUtc).HasDatabaseName("ix_leads_created_at");

        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.HasMany(l => l.Activities)
            .WithOne()
            .HasForeignKey(a => a.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(l => l.Activities).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(l => l.Remarks)
            .WithOne()
            .HasForeignKey(r => r.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(l => l.Remarks).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(l => l.Attachments)
            .WithOne()
            .HasForeignKey(a => a.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(l => l.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(l => l.DomainEvents);
    }
}
