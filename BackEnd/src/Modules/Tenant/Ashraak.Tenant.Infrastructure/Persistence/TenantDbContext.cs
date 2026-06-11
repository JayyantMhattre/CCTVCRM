using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Tenant.Domain.Aggregates.Tenant;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Tenant.Infrastructure.Persistence;

public sealed class TenantDbContext : BaseDbContext, IUnitOfWork
{
    private readonly ITenantContext _tenantContext;

    public TenantDbContext(DbContextOptions<TenantDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Domain.Aggregates.Tenant.Tenant> Tenants => Set<Domain.Aggregates.Tenant.Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tenant");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
