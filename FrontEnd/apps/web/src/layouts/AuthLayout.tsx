/**
 * AuthLayout — thin orchestration shim (T2).
 *
 * The auth shell's visual rendering now lives in the active theme adapter
 * (`theme/adapters/coreui/CoreUiAuthLayout`), orchestrated by
 * `PlatformAuthLayout` (which owns the loading/redirect gating). This file is
 * retained only as a backwards-compatible alias; new code should render
 * `PlatformAuthLayout` from `@/platform-ui` directly.
 */

import { PlatformAuthLayout } from '@/platform-ui';

export function AuthLayout() {
  return <PlatformAuthLayout />;
}
