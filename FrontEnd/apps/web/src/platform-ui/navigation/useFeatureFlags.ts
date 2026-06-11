/**
 * useFeatureFlags — platform feature-flag access for navigation visibility.
 *
 * T3/T7 preparation: navigation visibility rules may reference feature flags
 * (`NavigationVisibility.featureFlags`). There is no front-end feature-flag
 * backend yet, so every flag resolves to **enabled** — i.e. no behaviour change.
 *
 * When a real source arrives (config endpoint, env-driven flags, or a flag
 * service), replace the body of `isEnabled` here. Nothing else in the navigation
 * pipeline needs to change.
 */
export interface FeatureFlagApi {
  /** Returns true when the named feature flag is enabled. */
  isEnabled: (flag: string) => boolean;
}

export function useFeatureFlags(): FeatureFlagApi {
  function isEnabled(_flag: string): boolean {
    // No feature-flag backend yet — treat all flags as enabled.
    return true;
  }

  return { isEnabled };
}
