using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Users.Domain.Aggregates.UserProfile;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Users.Infrastructure.Persistence;

public sealed class UsersDbContext : BaseDbContext, IUnitOfWork
{
    private readonly ITenantContext _tenantContext;

    public UsersDbContext(DbContextOptions<UsersDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

        modelBuilder.Entity<UserProfile>()
            .HasQueryFilter(u => u.TenantId == _tenantContext.TenantId);

        base.OnModelCreating(modelBuilder);
    }
}
