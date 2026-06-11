using System.Text.Json;
using System.Text.Json.Nodes;
using Ashraak.Webhooks.Application.Abstractions;

namespace Ashraak.Webhooks.Infrastructure.Delivery;

internal sealed class WebhookPayloadBuilder : IWebhookPayloadBuilder
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string BuildEnvelopeJson(WebhookOutboundPayload payload)
    {
        JsonNode? dataNode;
        try
        {
            dataNode = JsonNode.Parse(payload.PayloadJson);
        }
        catch (JsonException)
        {
            dataNode = JsonValue.Create(payload.PayloadJson);
        }

        var envelope = new JsonObject
        {
            ["eventId"] = payload.EventId.ToString(),
            ["eventName"] = payload.EventName,
            ["version"] = payload.Version,
            ["occurredOnUtc"] = payload.OccurredOnUtc,
            ["tenantId"] = payload.TenantId.ToString(),
            ["correlationId"] = payload.CorrelationId,
            ["data"] = dataNode
        };

        return envelope.ToJsonString(SerializerOptions);
    }
}
