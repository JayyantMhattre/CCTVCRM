using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Infrastructure.Retry;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookRetryBackoffCalculatorTests
{
    private readonly WebhookRetryBackoffCalculator _calculator = new(
        Options.Create(new WebhookRetryOptions
        {
            MaxRetries = 5,
            RetryDelaysMinutes = [1, 5, 15, 60]
        }));

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 5)]
    [InlineData(3, 15)]
    [InlineData(4, 60)]
    public void GetDelayBeforeNextAttempt_UsesConfiguredSchedule(int attemptNumber, int expectedMinutes)
    {
        _calculator.GetDelayBeforeNextAttempt(attemptNumber)
            .Should().Be(TimeSpan.FromMinutes(expectedMinutes));
    }

    [Theory]
    [InlineData(1, 5, true)]
    [InlineData(4, 5, true)]
    [InlineData(5, 5, false)]
    public void CanRetry_RespectsMaxRetries(int attemptNumber, int maxRetries, bool expected)
    {
        _calculator.CanRetry(attemptNumber, maxRetries).Should().Be(expected);
    }
}
