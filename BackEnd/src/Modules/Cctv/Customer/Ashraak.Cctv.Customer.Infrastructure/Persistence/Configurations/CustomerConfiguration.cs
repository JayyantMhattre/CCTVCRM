using Ashraak.Cctv.Customer.Domain.Enums;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;
using CustomerId = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.CustomerId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<CustomerAggregate>
{
    public void Configure(EntityTypeBuilder<CustomerAggregate> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => CustomerId.From(value))
            .HasColumnName("id");

        builder.Property(c => c.CustomerNumber).HasMaxLength(32).IsRequired().HasColumnName("customer_number");
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired().HasColumnName("name");
        builder.Property(c => c.Email).HasMaxLength(320).IsRequired().HasColumnName("email");
        builder.Property(c => c.Phone).HasMaxLength(32).IsRequired().HasColumnName("phone");
        builder.Property(c => c.BillingAddress).HasMaxLength(500).IsRequired().HasColumnName("billing_address");
        builder.Property(c => c.City).HasMaxLength(100).IsRequired().HasColumnName("city");
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired()
            .HasColumnName("status");
        builder.Property(c => c.PortalUserId).HasColumnName("portal_user_id");
        builder.Property(c => c.SourceLeadId).HasColumnName("source_lead_id");
        builder.Property(c => c.CreatedAtUtc).IsRequired().HasColumnName("created_at");
        builder.Property(c => c.CreatedBy).IsRequired().HasColumnName("created_by");
        builder.Property(c => c.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");
        builder.Property(c => c.IsDeleted).IsRequired().HasColumnName("is_deleted");
        builder.Property(c => c.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        builder.HasIndex(c => c.CustomerNumber).IsUnique().HasDatabaseName("ux_customers_customer_number");
        builder.HasIndex(c => c.Status).HasDatabaseName("ix_customers_status");
        builder.HasIndex(c => c.PortalUserId).HasDatabaseName("ix_customers_portal_user_id");
        builder.HasIndex(c => c.SourceLeadId).HasDatabaseName("ix_customers_source_lead_id");
        builder.HasIndex(c => c.CreatedAtUtc).HasDatabaseName("ix_customers_created_at");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.DomainEvents);
    }
}
