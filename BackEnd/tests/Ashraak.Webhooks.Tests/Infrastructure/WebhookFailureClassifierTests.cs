using Ashraak.Webhooks.Domain.Enums;
using Ashraak.Webhooks.Infrastructure.Retry;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookFailureClassifierTests
{
    private readonly WebhookFailureClassifier _classifier = new();

    [Theory]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public void Classify_TransientStatusCodes_ReturnsTransient(int statusCode)
    {
        _classifier.Classify(statusCode, null).Should().Be(WebhookFailureType.Transient);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(410)]
    public void Classify_PermanentStatusCodes_ReturnsPermanent(int statusCode)
    {
        _classifier.Classify(statusCode, null).Should().Be(WebhookFailureType.Permanent);
    }

    [Fact]
    public void Classify_TimeoutMessage_ReturnsTransient()
    {
        _classifier.Classify(null, "The request was canceled due to timeout")
            .Should().Be(WebhookFailureType.Transient);
    }

    [Fact]
    public void Classify_DisabledSubscription_ReturnsPermanent()
    {
        _classifier.Classify(null, "Subscription is disabled.")
            .Should().Be(WebhookFailureType.Permanent);
    }
}
