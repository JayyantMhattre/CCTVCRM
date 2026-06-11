using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.ApiKeys.Infrastructure.Persistence;

public sealed class ApiKeysDbContext : BaseDbContext, IUnitOfWork
{
    private readonly ITenantContext _tenantContext;

    public ApiKeysDbContext(DbContextOptions<ApiKeysDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("apikeys");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiKeysDbContext).Assembly);

        modelBuilder.Entity<ApiKey>()
            .HasQueryFilter(k => k.TenantId == _tenantContext.TenantId);

        base.OnModelCreating(modelBuilder);
    }
}
