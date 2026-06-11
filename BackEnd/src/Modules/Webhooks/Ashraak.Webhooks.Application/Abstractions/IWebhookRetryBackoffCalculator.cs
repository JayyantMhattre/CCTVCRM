namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookRetryBackoffCalculator
{
    TimeSpan GetDelayBeforeNextAttempt(int currentAttemptNumber);

    bool CanRetry(int currentAttemptNumber, int maxRetries);
}
