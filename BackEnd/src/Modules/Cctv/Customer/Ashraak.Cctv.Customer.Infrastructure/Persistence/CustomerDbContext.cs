using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_customer</c>.</summary>
public sealed class CustomerDbContext : BaseDbContext, IUnitOfWork
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerAggregate> Customers => Set<CustomerAggregate>();
    public DbSet<SiteAggregate> Sites => Set<SiteAggregate>();
    public DbSet<SiteContact> SiteContacts => Set<SiteContact>();
    public DbSet<SiteDocument> SiteDocuments => Set<SiteDocument>();
    public DbSet<SiteAssetSummary> SiteAssetSummaries => Set<SiteAssetSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_customer");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
