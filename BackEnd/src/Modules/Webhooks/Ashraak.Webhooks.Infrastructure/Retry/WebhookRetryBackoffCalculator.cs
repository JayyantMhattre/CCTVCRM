using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Infrastructure.Retry;

internal sealed class WebhookRetryBackoffCalculator(IOptions<WebhookRetryOptions> retryOptions)
    : IWebhookRetryBackoffCalculator
{
    public TimeSpan GetDelayBeforeNextAttempt(int currentAttemptNumber)
    {
        var options = retryOptions.Value;
        var delays = options.RetryDelaysMinutes;

        if (delays is { Length: > 0 })
        {
            var index = currentAttemptNumber - 1;
            if (index >= 0 && index < delays.Length)
                return TimeSpan.FromMinutes(Math.Max(0, delays[index]));

            if (index >= delays.Length)
                return TimeSpan.FromMinutes(delays[^1]);
        }

        var multiplier = Math.Max(1, options.BackoffMultiplier);
        var minutes = options.InitialDelayMinutes * Math.Pow(multiplier, currentAttemptNumber - 1);
        return TimeSpan.FromMinutes(Math.Min(minutes, 24 * 60));
    }

    public bool CanRetry(int currentAttemptNumber, int maxRetries) =>
        currentAttemptNumber < Math.Max(1, maxRetries);
}
