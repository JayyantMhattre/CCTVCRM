using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using ScheduleAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Schedule.ServiceSchedule;
using VisitAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Visit.ServiceVisit;

namespace Ashraak.Cctv.Service.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_service</c>.</summary>
public sealed class ServiceDbContext : BaseDbContext, IUnitOfWork
{
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) { }

    public DbSet<ScheduleAggregate> ServiceSchedules => Set<ScheduleAggregate>();
    public DbSet<EngineerAssignment> EngineerAssignments => Set<EngineerAssignment>();
    public DbSet<VisitAggregate> ServiceVisits => Set<VisitAggregate>();
    public DbSet<VisitPhoto> VisitPhotos => Set<VisitPhoto>();
    public DbSet<VisitLocation> VisitLocations => Set<VisitLocation>();
    public DbSet<VisitSignature> VisitSignatures => Set<VisitSignature>();
    public DbSet<VisitApproval> VisitApprovals => Set<VisitApproval>();
    public DbSet<VisitAttachment> VisitAttachments => Set<VisitAttachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_service");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
