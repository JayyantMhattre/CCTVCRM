using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.Webhooks.Application.Mapping;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Repositories;

internal sealed class WebhookSubscriptionRepository(WebhooksDbContext context)
    : IWebhookSubscriptionStore, IWebhookSubscriptionRepository
{
    public Task<WebhookSubscription?> GetByIdAsync(
        WebhookSubscriptionId id,
        CancellationToken cancellationToken = default) =>
        context.WebhookSubscriptions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default) =>
        context.WebhookSubscriptions.AnyAsync(
            s => s.TenantId == tenantId && s.Name == name.Trim(),
            cancellationToken);

    public Task<bool> ExistsByNameExcludingIdAsync(
        Guid tenantId,
        string name,
        WebhookSubscriptionId excludeId,
        CancellationToken cancellationToken = default) =>
        context.WebhookSubscriptions.AnyAsync(
            s => s.TenantId == tenantId && s.Name == name.Trim() && s.Id != excludeId,
            cancellationToken);

    public async Task<IReadOnlyList<WebhookSubscription>> GetEnabledForEventAsync(
        Guid tenantId,
        string eventName,
        CancellationToken cancellationToken = default)
    {
        var normalized = eventName.Trim().ToLowerInvariant();
        var subscriptions = await context.WebhookSubscriptions
            .IgnoreQueryFilters()
            .Where(s => s.TenantId == tenantId && s.Enabled)
            .ToListAsync(cancellationToken);

        return subscriptions.Where(s => s.IsSubscribedToEvent(normalized)).ToList();
    }

    public void Add(WebhookSubscription subscription) => context.WebhookSubscriptions.Add(subscription);

    public void Update(WebhookSubscription subscription) => context.WebhookSubscriptions.Update(subscription);

    public async Task<WebhookSubscriptionContract?> GetByIdAsync(
        Guid tenantId,
        Guid subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.WebhookSubscriptions
            .FirstOrDefaultAsync(s => s.Id == WebhookSubscriptionId.From(subscriptionId) && s.TenantId == tenantId, cancellationToken);

        return entity is null ? null : WebhookSubscriptionMapper.ToContract(entity);
    }

    public async Task<IReadOnlyList<WebhookSubscriptionContract>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var items = await context.WebhookSubscriptions
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return items.Select(WebhookSubscriptionMapper.ToContract).ToList();
    }
}
