using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.ApiKeys.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ashraak.Api.Health;

/// <summary>Verifies API Keys module DI registration.</summary>
internal sealed class ApiKeysHealthCheck(IServiceProvider services) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = services.GetRequiredService<IApiKeyRepository>();
            _ = services.GetRequiredService<IApiKeyValidator>();
            return Task.FromResult(HealthCheckResult.Healthy("API Keys module registered."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "API Keys module is not healthy.", ex));
        }
    }
}
