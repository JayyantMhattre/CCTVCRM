using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Cctv.Engineer.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_engineer</c>.</summary>
public sealed class EngineerDbContext : BaseDbContext, IUnitOfWork
{
    public EngineerDbContext(DbContextOptions<EngineerDbContext> options) : base(options) { }

    public DbSet<EngineerAggregate> Engineers => Set<EngineerAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_engineer");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EngineerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
