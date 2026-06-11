/**
 * platform-ui/navigation — the platform's navigation API.
 *
 * The platform owns the menu, its grouping, and all visibility decisions
 * (roles / permissions / feature flags). Themes only render the resolved model.
 */

export * from './models';
export { NAVIGATION_CONFIG } from './navigationConfig';
export { usePlatformNav } from './usePlatformNav';
export { useFeatureFlags, type FeatureFlagApi } from './useFeatureFlags';
export {
  PlatformNavigationProvider,
  useNavigationModel,
} from './PlatformNavigationProvider';
export { PlatformNavRenderer } from './PlatformNavRenderer';
