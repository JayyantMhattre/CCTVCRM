using System.Text.Json.Nodes;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Infrastructure.Delivery;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookPayloadBuilderTests
{
    [Fact]
    public void BuildEnvelopeJson_UsesStandardShape()
    {
        var builder = new WebhookPayloadBuilder();
        var tenantId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var json = builder.BuildEnvelopeJson(new WebhookOutboundPayload(
            eventId,
            "user.created",
            "v1",
            DateTime.UtcNow,
            tenantId,
            "corr-123",
            """{"email":"a@test.com"}"""));

        var node = JsonNode.Parse(json)!.AsObject();
        node["eventId"]!.GetValue<string>().Should().Be(eventId.ToString());
        node["eventName"]!.GetValue<string>().Should().Be("user.created");
        node["version"]!.GetValue<string>().Should().Be("v1");
        node["tenantId"]!.GetValue<string>().Should().Be(tenantId.ToString());
        node["correlationId"]!.GetValue<string>().Should().Be("corr-123");
        node["data"]!["email"]!.GetValue<string>().Should().Be("a@test.com");
    }
}
