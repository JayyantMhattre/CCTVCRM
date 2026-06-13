using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ashraak.Cctv.Service.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core migrations (no host/Redis required).</summary>
public sealed class ServiceDbContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
{
    public ServiceDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SERVICE_MIGRATION_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ashraak;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_service");
        });

        return new ServiceDbContext(optionsBuilder.Options);
    }
}
