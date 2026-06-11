using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery.Events;
using Ashraak.Webhooks.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Domain;

public sealed class WebhookDeliveryRetryTests
{
    [Fact]
    public void ScheduleRetry_SetsRetryingStatusAndRaisesEvents()
    {
        var delivery = CreateSample();
        delivery.ClearDomainEvents();

        delivery.ScheduleRetry(DateTime.UtcNow.AddMinutes(5), 503, "HTTP 503");

        delivery.Status.Should().Be(WebhookDeliveryStatus.Retrying);
        delivery.RetryCount.Should().Be(1);
        delivery.NextRetryOnUtc.Should().NotBeNull();
        delivery.DomainEvents.Should().Contain(e => e is WebhookRetryScheduledDomainEvent);
        delivery.DomainEvents.Should().Contain(e => e is WebhookRetryFailedDomainEvent);
    }

    [Fact]
    public void BeginRetryAttempt_IncrementsAttemptNumber()
    {
        var delivery = CreateSample();
        delivery.ScheduleRetry(DateTime.UtcNow.AddMinutes(1), 500, "error");
        delivery.ClearDomainEvents();

        delivery.BeginRetryAttempt();

        delivery.Status.Should().Be(WebhookDeliveryStatus.Pending);
        delivery.AttemptNumber.Should().Be(2);
        delivery.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookRetryAttemptedDomainEvent>();
    }

    [Fact]
    public void MarkSucceeded_OnRetry_RaisesRetrySucceededEvent()
    {
        var delivery = CreateSample();
        delivery.BeginRetryAttempt();
        delivery.ClearDomainEvents();

        delivery.MarkSucceeded(200, "ok");

        delivery.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookRetrySucceededDomainEvent>();
    }

    [Fact]
    public void MarkDeadLettered_SetsDeadLetteredStatus()
    {
        var delivery = CreateSample();

        delivery.MarkDeadLettered(503, "exhausted");

        delivery.Status.Should().Be(WebhookDeliveryStatus.DeadLettered);
    }

    private static WebhookDelivery CreateSample() =>
        WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user.created",
            "v1",
            "corr-1",
            "{}");
}
