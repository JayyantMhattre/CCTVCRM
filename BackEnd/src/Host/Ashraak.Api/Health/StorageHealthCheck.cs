using Ashraak.Files.Infrastructure.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Ashraak.Api.Health;

/// <summary>Verifies Files module storage provider connectivity.</summary>
internal sealed class StorageHealthCheck(
    IServiceProvider services,
    IOptions<StorageOptions> options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = services.CreateScope();
            var indicator = scope.ServiceProvider.GetRequiredService<IStorageHealthIndicator>();
            var ok = await indicator.CheckHealthAsync(cancellationToken);
            return ok
                ? HealthCheckResult.Healthy($"Storage provider '{options.Value.Provider}' is reachable.")
                : HealthCheckResult.Degraded($"Storage provider '{options.Value.Provider}' health check failed.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Storage provider is not available.", ex);
        }
    }
}
