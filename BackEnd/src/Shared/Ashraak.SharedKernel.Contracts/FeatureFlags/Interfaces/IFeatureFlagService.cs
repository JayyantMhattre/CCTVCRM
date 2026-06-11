namespace Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;

/// <summary>
/// Minimal feature-flag contract for plan/tenant gating (foundation only).
/// </summary>
public interface IFeatureFlagService
{
    /// <summary>
    /// Returns whether the named feature is enabled globally or for an optional tenant scope.
    /// </summary>
    /// <param name="featureName">Stable feature key (e.g. <c>notifications.email</c>).</param>
    /// <param name="tenantId">Optional tenant scope; when null, evaluates global config only.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<bool> IsEnabledAsync(
        string featureName,
        Guid? tenantId = null,
        CancellationToken cancellationToken = default);
}
