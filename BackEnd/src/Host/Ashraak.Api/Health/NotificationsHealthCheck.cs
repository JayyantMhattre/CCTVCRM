using Ashraak.SharedKernel.Contracts.Notifications.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Ashraak.Notifications.Infrastructure;

namespace Ashraak.Api.Health;

/// <summary>Verifies Notifications module DI and template configuration.</summary>
internal sealed class NotificationsHealthCheck(
    IServiceProvider services,
    IOptions<NotificationOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _ = services.GetRequiredService<INotificationService>();
            var opts = options.Value;
            var templatesPath = opts.TemplatesPath;
            if (string.IsNullOrWhiteSpace(templatesPath))
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    "Notifications provider registered but TemplatesPath is empty."));
            }

            var fullPath = Path.IsPathRooted(templatesPath)
                ? templatesPath
                : Path.Combine(AppContext.BaseDirectory, templatesPath);

            if (!Directory.Exists(fullPath))
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Email templates directory not found: {fullPath}"));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Notifications module ready (provider: {opts.Provider ?? "console"})."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Notifications module is not registered.", ex));
        }
    }
}
