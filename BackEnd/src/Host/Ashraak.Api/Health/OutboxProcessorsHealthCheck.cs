using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Ashraak.Api.Health;

/// <summary>Verifies outbox hosted processors and configuration.</summary>
internal sealed class OutboxProcessorsHealthCheck(
    IServiceProvider services,
    IOptions<OutboxProcessorOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var hosted = services.GetServices<IHostedService>().ToList();
        var outboxWorkers = hosted.Count(s =>
            s.GetType().IsGenericType &&
            s.GetType().GetGenericTypeDefinition() == typeof(OutboxProcessorHostedService<>));

        if (outboxWorkers < 5)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"Expected 5 outbox processors (Auth, Tenant, Users, Files, Webhooks); found {outboxWorkers}."));
        }

        var opts = options.Value;
        if (opts.BatchSize <= 0 || opts.PollInterval <= TimeSpan.Zero)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Outbox configuration is invalid (BatchSize and PollInterval must be positive)."));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Outbox processors running ({outboxWorkers} workers, poll {opts.PollInterval.TotalSeconds}s)."));
    }
}
