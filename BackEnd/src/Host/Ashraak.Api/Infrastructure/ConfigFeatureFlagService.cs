using Ashraak.Api.Options;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Microsoft.Extensions.Options;

namespace Ashraak.Api.Infrastructure;

/// <summary>Configuration-backed <see cref="IFeatureFlagService"/> (foundation only).</summary>
internal sealed class ConfigFeatureFlagService(IOptionsMonitor<FeatureFlagOptions> options) : IFeatureFlagService
{
    public Task<bool> IsEnabledAsync(
        string featureName,
        Guid? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(featureName))
            return Task.FromResult(false);

        var config = options.CurrentValue;

        if (tenantId is { } tid &&
            config.TenantOverrides.TryGetValue(tid.ToString("D"), out var tenantFlags) &&
            tenantFlags.TryGetValue(featureName, out var tenantEnabled))
            return Task.FromResult(tenantEnabled);

        if (tenantId is { } tidN &&
            config.TenantOverrides.TryGetValue(tidN.ToString("N"), out tenantFlags) &&
            tenantFlags.TryGetValue(featureName, out tenantEnabled))
            return Task.FromResult(tenantEnabled);

        if (config.Flags.TryGetValue(featureName, out var globalEnabled))
            return Task.FromResult(globalEnabled);

        return Task.FromResult(false);
    }
}
