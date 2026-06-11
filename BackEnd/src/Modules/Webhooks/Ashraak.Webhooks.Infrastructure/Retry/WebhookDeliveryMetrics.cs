using System.Diagnostics.Metrics;

namespace Ashraak.Webhooks.Infrastructure.Retry;

internal sealed class WebhookDeliveryMetrics
{
    public const string MeterName = "Ashraak.Webhooks.Delivery";

    private static readonly Meter Meter = new(MeterName);

    private readonly Counter<long> _deliveryAttempts;
    private readonly Counter<long> _deliverySuccesses;
    private readonly Counter<long> _deliveryFailures;
    private readonly Counter<long> _retriesScheduled;
    private readonly Counter<long> _deadLettersCreated;
    private readonly Counter<long> _deadLettersReplayed;
    private readonly Histogram<double> _deliveryDurationMs;

    public WebhookDeliveryMetrics()
    {
        _deliveryAttempts = Meter.CreateCounter<long>("webhook.delivery.attempts");
        _deliverySuccesses = Meter.CreateCounter<long>("webhook.delivery.successes");
        _deliveryFailures = Meter.CreateCounter<long>("webhook.delivery.failures");
        _retriesScheduled = Meter.CreateCounter<long>("webhook.retry.scheduled");
        _deadLettersCreated = Meter.CreateCounter<long>("webhook.dlq.created");
        _deadLettersReplayed = Meter.CreateCounter<long>("webhook.dlq.replayed");
        _deliveryDurationMs = Meter.CreateHistogram<double>("webhook.delivery.duration_ms");
    }

    public void RecordAttempt() => _deliveryAttempts.Add(1);

    public void RecordSuccess(double durationMs)
    {
        _deliverySuccesses.Add(1);
        _deliveryDurationMs.Record(durationMs);
    }

    public void RecordFailure() => _deliveryFailures.Add(1);

    public void RecordRetryScheduled() => _retriesScheduled.Add(1);

    public void RecordDeadLetterCreated() => _deadLettersCreated.Add(1);

    public void RecordDeadLetterReplayed() => _deadLettersReplayed.Add(1);
}
