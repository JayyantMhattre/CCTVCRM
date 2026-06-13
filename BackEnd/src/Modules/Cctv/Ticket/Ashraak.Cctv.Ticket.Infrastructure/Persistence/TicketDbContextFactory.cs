using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence;

/// <summary>Design-time factory for EF Core migrations.</summary>
public sealed class TicketDbContextFactory : IDesignTimeDbContextFactory<TicketDbContext>
{
    public TicketDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TicketDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=ashraak;Username=postgres;Password=postgres",
            npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_ticket"));

        return new TicketDbContext(optionsBuilder.Options);
    }
}
