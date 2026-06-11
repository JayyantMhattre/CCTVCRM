using Ashraak.Api.Health;
using Ashraak.Api.Infrastructure;
using Ashraak.Api.Middleware;
using Ashraak.Api.Options;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ashraak.Api.Extensions;

/// <summary>
/// Host-level platform services: rate limiting, correlation, health, env validation, feature flags.
/// </summary>
internal static class HostPlatformExtensions
{
    public static IServiceCollection AddHostPlatformServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));
        services.Configure<FeatureFlagOptions>(configuration.GetSection(FeatureFlagOptions.SectionName));
        services.AddSingleton<IFeatureFlagService, ConfigFeatureFlagService>();
        return services;
    }

    /// <summary>Adds platform health checks (notifications, outbox processors).</summary>
    public static IHealthChecksBuilder AddPlatformHealthChecks(this IHealthChecksBuilder builder) =>
        builder
            .AddCheck<NotificationsHealthCheck>("notifications", tags: ["ready", "platform"])
            .AddCheck<OutboxProcessorsHealthCheck>("outbox_processors", tags: ["ready", "platform"])
            .AddCheck<StorageHealthCheck>("storage", tags: ["ready", "platform"])
            .AddCheck<WebhooksHealthCheck>("webhooks", tags: ["ready", "platform"])
            .AddCheck<ApiKeysHealthCheck>("apikeys", tags: ["ready", "platform"]);

    public static WebApplication MapPlatformHealthChecks(this WebApplication app)
    {
        var jsonOptions = new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        };

        app.MapHealthChecks("/health", jsonOptions);
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
