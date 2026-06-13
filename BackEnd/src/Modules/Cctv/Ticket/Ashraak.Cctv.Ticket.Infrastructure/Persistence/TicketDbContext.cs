using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_ticket</c>.</summary>
public sealed class TicketDbContext : BaseDbContext, IUnitOfWork
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options)
        : base(options)
    {
    }

    public DbSet<TicketAggregate> Tickets => Set<TicketAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_ticket");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
