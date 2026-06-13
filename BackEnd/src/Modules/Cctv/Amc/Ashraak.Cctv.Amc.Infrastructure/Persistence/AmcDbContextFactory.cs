using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ashraak.Cctv.Amc.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core migrations.</summary>
public sealed class AmcDbContextFactory : IDesignTimeDbContextFactory<AmcDbContext>
{
    public AmcDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AmcDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=ashraak;Username=postgres;Password=postgres",
            npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_amc"));

        return new AmcDbContext(optionsBuilder.Options);
    }
}
