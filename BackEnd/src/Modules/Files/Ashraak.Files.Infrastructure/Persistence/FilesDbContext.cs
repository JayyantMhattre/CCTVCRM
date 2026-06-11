using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Files.Domain.Aggregates.FileRecord;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Files.Infrastructure.Persistence;

public sealed class FilesDbContext : BaseDbContext, IUnitOfWork
{
    private readonly ITenantContext _tenantContext;

    public FilesDbContext(DbContextOptions<FilesDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<FileRecord> FileRecords => Set<FileRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("files");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesDbContext).Assembly);

        modelBuilder.Entity<FileRecord>()
            .HasQueryFilter(f => f.TenantId == _tenantContext.TenantId);

        base.OnModelCreating(modelBuilder);
    }
}
