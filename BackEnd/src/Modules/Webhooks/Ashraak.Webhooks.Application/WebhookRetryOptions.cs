namespace Ashraak.Webhooks.Application;

public sealed class WebhookRetryOptions
{
    public const string SectionName = "WebhookRetry";

    public bool Enabled { get; set; } = true;

    public int MaxRetries { get; set; } = 5;

    public int InitialDelayMinutes { get; set; } = 1;

    public double BackoffMultiplier { get; set; } = 5;

    /// <summary>Minutes to wait before each retry attempt (attempt 2, 3, 4, 5). Configurable.</summary>
    public int[] RetryDelaysMinutes { get; set; } = [1, 5, 15, 60];

    public int PollIntervalSeconds { get; set; } = 30;
}
