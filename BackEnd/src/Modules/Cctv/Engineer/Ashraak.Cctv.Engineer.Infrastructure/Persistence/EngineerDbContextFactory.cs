using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ashraak.Cctv.Engineer.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core migrations.</summary>
public sealed class EngineerDbContextFactory : IDesignTimeDbContextFactory<EngineerDbContext>
{
    public EngineerDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ENGINEER_MIGRATION_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ashraak;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<EngineerDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_engineer");
        });

        return new EngineerDbContext(optionsBuilder.Options);
    }
}
