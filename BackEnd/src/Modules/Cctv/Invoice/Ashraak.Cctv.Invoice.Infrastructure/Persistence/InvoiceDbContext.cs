using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.SharedKernel.Interfaces;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Cctv.Invoice.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>cctv_invoice</c>.</summary>
public sealed class InvoiceDbContext : BaseDbContext, IUnitOfWork
{
    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<InvoiceAggregate> Invoices => Set<InvoiceAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cctv_invoice");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
