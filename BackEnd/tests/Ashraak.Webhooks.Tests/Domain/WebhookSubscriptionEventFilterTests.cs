using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Domain;

public sealed class WebhookSubscriptionEventFilterTests
{
    [Fact]
    public void IsSubscribedToEvent_EmptyList_MatchesAll()
    {
        var subscription = WebhookSubscription.Create(
            WebhookSubscriptionId.New(),
            Guid.NewGuid(),
            "Hook",
            "https://example.com/hook",
            "secret",
            Guid.NewGuid());

        subscription.IsSubscribedToEvent("user.created").Should().BeTrue();
    }

    [Fact]
    public void IsSubscribedToEvent_FilteredList_MatchesOnlyListed()
    {
        var subscription = WebhookSubscription.Create(
            WebhookSubscriptionId.New(),
            Guid.NewGuid(),
            "Hook",
            "https://example.com/hook",
            "secret",
            Guid.NewGuid(),
            ["user.created"]);

        subscription.IsSubscribedToEvent("user.created").Should().BeTrue();
        subscription.IsSubscribedToEvent("tenant.created").Should().BeFalse();
    }
}
