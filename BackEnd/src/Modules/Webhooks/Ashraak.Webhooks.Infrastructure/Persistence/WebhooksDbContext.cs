using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Webhooks.Infrastructure.Persistence;

public sealed class WebhooksDbContext : BaseDbContext, IUnitOfWork
{
    private readonly ITenantContext _tenantContext;

    public WebhooksDbContext(DbContextOptions<WebhooksDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    public DbSet<WebhookEventDefinition> WebhookEventDefinitions => Set<WebhookEventDefinition>();
    public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();
    public DbSet<WebhookDeadLetter> WebhookDeadLetters => Set<WebhookDeadLetter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("webhooks");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebhooksDbContext).Assembly);

        modelBuilder.Entity<WebhookSubscription>()
            .HasQueryFilter(s => s.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<WebhookDelivery>()
            .HasQueryFilter(d => d.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<WebhookDeadLetter>()
            .HasQueryFilter(d => d.TenantId == _tenantContext.TenantId);

        base.OnModelCreating(modelBuilder);
    }
}
