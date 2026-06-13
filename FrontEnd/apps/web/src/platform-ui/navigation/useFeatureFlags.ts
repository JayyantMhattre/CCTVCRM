/**
 * useFeatureFlags — platform feature-flag access for navigation visibility.
 *
 * Resolves flags from static defaults (CCTV placeholders) until a tenant
 * flag API or config endpoint is wired. Unknown flags default to enabled
 * to preserve pre-CCTV platform behaviour.
 */
import {
  CCTV_FEATURE_FLAG_DEFAULTS,
  type CctvFeatureFlagKey,
} from '@/modules/cctv/featureFlags/cctvFeatureFlags';

export interface FeatureFlagApi {
  /** Returns true when the named feature flag is enabled. */
  isEnabled: (flag: string) => boolean;
}

export function useFeatureFlags(): FeatureFlagApi {
  function isEnabled(flag: string): boolean {
    if (flag in CCTV_FEATURE_FLAG_DEFAULTS) {
      return CCTV_FEATURE_FLAG_DEFAULTS[flag as CctvFeatureFlagKey];
    }

    return true;
  }

  return { isEnabled };
}
