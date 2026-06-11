namespace Ashraak.Webhooks.Application.Abstractions;

public sealed record WebhookOutboundPayload(
    Guid EventId,
    string EventName,
    string Version,
    DateTime OccurredOnUtc,
    Guid TenantId,
    string? CorrelationId,
    string PayloadJson);

public interface IWebhookPayloadBuilder
{
    string BuildEnvelopeJson(WebhookOutboundPayload payload);
}
