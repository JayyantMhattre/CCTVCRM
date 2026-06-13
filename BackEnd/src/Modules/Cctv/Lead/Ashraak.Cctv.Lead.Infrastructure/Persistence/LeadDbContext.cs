using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadActivity;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadRemark;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_lead</c>.</summary>
public sealed class LeadDbContext : BaseDbContext, IUnitOfWork
{
    public LeadDbContext(DbContextOptions<LeadDbContext> options)
        : base(options)
    {
    }

    public DbSet<LeadAggregate> Leads => Set<LeadAggregate>();
    public DbSet<LeadActivity> LeadActivities => Set<LeadActivity>();
    public DbSet<LeadRemark> LeadRemarks => Set<LeadRemark>();
    public DbSet<LeadAttachment> LeadAttachments => Set<LeadAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_lead");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LeadDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
