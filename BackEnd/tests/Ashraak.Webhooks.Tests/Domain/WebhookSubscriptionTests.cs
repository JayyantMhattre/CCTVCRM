using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription.Events;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Domain;

public sealed class WebhookSubscriptionTests
{
    [Fact]
    public void Create_RaisesCreatedDomainEvent()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var subscription = WebhookSubscription.Create(
            WebhookSubscriptionId.New(),
            tenantId,
            "CRM Hook",
            "https://example.com/hooks",
            "protected-secret",
            userId);

        subscription.Enabled.Should().BeTrue();
        subscription.DomainEvents.Should().HaveCount(1);
        subscription.DomainEvents[0].Should().BeOfType<WebhookSubscriptionCreatedDomainEvent>();
    }

    [Fact]
    public void Disable_RaisesDisabledDomainEvent()
    {
        var subscription = CreateSample();
        subscription.ClearDomainEvents();

        subscription.Disable(Guid.NewGuid());

        subscription.Enabled.Should().BeFalse();
        subscription.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookSubscriptionDisabledDomainEvent>();
    }

    [Fact]
    public void RotateSecret_RaisesRotatedDomainEvent()
    {
        var subscription = CreateSample();
        subscription.ClearDomainEvents();

        subscription.RotateSecret("new-protected", Guid.NewGuid());

        subscription.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WebhookSecretRotatedDomainEvent>();
    }

    private static WebhookSubscription CreateSample() =>
        WebhookSubscription.Create(
            WebhookSubscriptionId.New(),
            Guid.NewGuid(),
            "Hook",
            "https://example.com/hook",
            "secret",
            Guid.NewGuid());
}
