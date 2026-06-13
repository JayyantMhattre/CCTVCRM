using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ashraak.Cctv.Customer.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core migrations (no host/Redis required).</summary>
public sealed class CustomerDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CUSTOMER_MIGRATION_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ashraak;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_customer");
        });

        return new CustomerDbContext(optionsBuilder.Options);
    }
}
