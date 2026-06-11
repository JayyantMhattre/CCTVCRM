namespace Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

/// <summary>
/// Cross-module payload for requesting a webhook-eligible event enter the platform outbox.
/// Delivery is asynchronous (W2+); W1 only enqueues <see cref="Events.WebhookRequestedEvent"/>.
/// </summary>
/// <param name="TenantId">Tenant that owns the event.</param>
/// <param name="EventName">Catalog name (e.g. <c>user.created</c>).</param>
/// <param name="Version">Schema version (e.g. <c>v1</c>).</param>
/// <param name="PayloadJson">Serialised event data (JSON object).</param>
/// <param name="CorrelationId">Optional platform correlation id.</param>
public sealed record WebhookEventContract(
    Guid TenantId,
    string EventName,
    string Version,
    string PayloadJson,
    string? CorrelationId = null);
