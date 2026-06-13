using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ashraak.Cctv.Lead.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core migrations (no host/Redis required).</summary>
public sealed class LeadDbContextFactory : IDesignTimeDbContextFactory<LeadDbContext>
{
    public LeadDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("LEAD_MIGRATION_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ashraak;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LeadDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_lead");
        });

        return new LeadDbContext(optionsBuilder.Options);
    }
}
