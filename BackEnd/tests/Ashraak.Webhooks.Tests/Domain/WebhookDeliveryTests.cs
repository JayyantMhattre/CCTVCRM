using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery.Events;
using Ashraak.Webhooks.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Domain;

public sealed class WebhookDeliveryTests
{
    [Fact]
    public void CreatePending_RaisesRequestedEvent()
    {
        var delivery = WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user.created",
            "v1",
            "corr-1",
            "{}");

        delivery.Status.Should().Be(WebhookDeliveryStatus.Pending);
        delivery.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookDeliveryRequestedDomainEvent>();
    }

    [Fact]
    public void MarkSucceeded_RaisesSucceededEvent()
    {
        var delivery = CreateSample();
        delivery.ClearDomainEvents();

        delivery.MarkSucceeded(200, "ok");

        delivery.Status.Should().Be(WebhookDeliveryStatus.Succeeded);
        delivery.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookDeliverySucceededDomainEvent>();
    }

    [Fact]
    public void MarkFailed_RaisesFailedEvent()
    {
        var delivery = CreateSample();
        delivery.ClearDomainEvents();

        delivery.MarkFailed(500, "error", "HTTP 500");

        delivery.Status.Should().Be(WebhookDeliveryStatus.Failed);
        delivery.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookDeliveryFailedDomainEvent>();
    }

    private static WebhookDelivery CreateSample() =>
        WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user.created",
            "v1",
            null,
            "{}");
}
