namespace Ashraak.Api.Options;

/// <summary>
/// Configuration-backed feature flags (global and optional per-tenant overrides).
/// </summary>
public sealed class FeatureFlagOptions
{
    public const string SectionName = "Features";

    /// <summary>Global feature switches keyed by feature name.</summary>
    public Dictionary<string, bool> Flags { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Per-tenant overrides: tenantId (string) → feature name → enabled.
    /// </summary>
    public Dictionary<string, Dictionary<string, bool>> TenantOverrides { get; set; } = new();
}
