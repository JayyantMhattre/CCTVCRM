using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.SharedKernel.Interfaces;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_amc</c>.</summary>
public sealed class AmcDbContext : BaseDbContext, IUnitOfWork
{
    public AmcDbContext(DbContextOptions<AmcDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlanAggregate> AmcPlans => Set<PlanAggregate>();
    public DbSet<AmcPlanVersion> AmcPlanVersions => Set<AmcPlanVersion>();
    public DbSet<ContractAggregate> AmcContracts => Set<ContractAggregate>();
    public DbSet<AmcContractTerm> AmcContractTerms => Set<AmcContractTerm>();
    public DbSet<AmcContractDocument> AmcContractDocuments => Set<AmcContractDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_amc");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AmcDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
