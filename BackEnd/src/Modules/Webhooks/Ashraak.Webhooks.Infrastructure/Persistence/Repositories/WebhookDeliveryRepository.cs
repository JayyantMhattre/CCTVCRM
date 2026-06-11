using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.Webhooks.Application.Mapping;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;
using Ashraak.Webhooks.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Webhooks.Infrastructure.Persistence.Repositories;

internal sealed class WebhookDeliveryRepository(WebhooksDbContext context)
    : IWebhookDeliveryStore, IWebhookDeliveryRepository
{
    public Task<WebhookDelivery?> GetByIdAsync(
        WebhookDeliveryId id,
        CancellationToken cancellationToken = default) =>
        context.WebhookDeliveries
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<IReadOnlyList<WebhookDelivery>> GetHistoryAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        WebhookDeliveryStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = context.WebhookDeliveries
            .Where(d => d.TenantId == tenantId)
            .AsQueryable();

        if (subscriptionId.HasValue)
            query = query.Where(d => d.SubscriptionId == subscriptionId.Value);

        if (!string.IsNullOrWhiteSpace(eventName))
            query = query.Where(d => d.EventName == eventName.Trim().ToLowerInvariant());

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        if (fromUtc.HasValue)
            query = query.Where(d => d.StartedOnUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(d => d.StartedOnUtc <= toUtc.Value);

        return await query
            .OrderByDescending(d => d.StartedOnUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WebhookDelivery>> GetDueForRetryAsync(
        DateTime utcNow,
        int batchSize,
        CancellationToken cancellationToken = default) =>
        await context.WebhookDeliveries
            .IgnoreQueryFilters()
            .Where(d =>
                d.Status == WebhookDeliveryStatus.Retrying
                && d.NextRetryOnUtc != null
                && d.NextRetryOnUtc <= utcNow)
            .OrderBy(d => d.NextRetryOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

    public Task<WebhookDelivery?> GetByIdForTenantAsync(
        WebhookDeliveryId id,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        context.WebhookDeliveries
            .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId, cancellationToken);

    public void Add(WebhookDelivery delivery) => context.WebhookDeliveries.Add(delivery);

    public void Update(WebhookDelivery delivery) => context.WebhookDeliveries.Update(delivery);

    public async Task<WebhookDeliveryContract?> GetByIdAsync(
        Guid tenantId,
        Guid deliveryId,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.WebhookDeliveries
            .FirstOrDefaultAsync(
                d => d.Id == WebhookDeliveryId.From(deliveryId) && d.TenantId == tenantId,
                cancellationToken);

        return entity is null ? null : WebhookDeliveryMapper.ToContract(entity);
    }

    public async Task<IReadOnlyList<WebhookDeliveryContract>> GetHistoryAsync(
        Guid tenantId,
        Guid? subscriptionId,
        string? eventName,
        string? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var items = await GetHistoryAsync(
            tenantId,
            subscriptionId,
            eventName,
            WebhookDeliveryMapper.ParseStatus(status),
            fromUtc,
            toUtc,
            limit,
            cancellationToken);

        return items.Select(WebhookDeliveryMapper.ToContract).ToList();
    }
}
