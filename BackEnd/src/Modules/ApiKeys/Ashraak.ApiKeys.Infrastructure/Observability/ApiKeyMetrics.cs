using System.Diagnostics.Metrics;

namespace Ashraak.ApiKeys.Infrastructure.Observability;

public sealed class ApiKeyMetrics
{
    public const string MeterName = "Ashraak.ApiKeys";

    private readonly Counter<long> _requests;
    private readonly Counter<long> _failures;
    private readonly Counter<long> _created;
    private readonly Counter<long> _revoked;

    public ApiKeyMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _requests = meter.CreateCounter<long>("apikeys.requests");
        _failures = meter.CreateCounter<long>("apikeys.failures");
        _created = meter.CreateCounter<long>("apikeys.created");
        _revoked = meter.CreateCounter<long>("apikeys.revoked");
    }

    public void RecordRequest(Guid tenantId, bool success)
    {
        var tags = new KeyValuePair<string, object?>("tenant_id", tenantId);
        _requests.Add(1, tags);
        if (!success)
            _failures.Add(1, tags);
    }

    public void RecordCreated(Guid tenantId) =>
        _created.Add(1, new KeyValuePair<string, object?>("tenant_id", tenantId));

    public void RecordRevoked(Guid tenantId) =>
        _revoked.Add(1, new KeyValuePair<string, object?>("tenant_id", tenantId));
}
