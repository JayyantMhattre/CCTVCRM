/**
 * AppLayout — thin orchestration shim (T2).
 *
 * The authenticated shell's visual rendering now lives in the active theme
 * adapter (`theme/adapters/coreui/CoreUiLayout`), orchestrated by
 * `PlatformLayout`. This file is retained only as a backwards-compatible alias;
 * new code should render `PlatformLayout` from `@/platform-ui` directly.
 */

import { PlatformLayout } from '@/platform-ui';

export function AppLayout() {
  return <PlatformLayout />;
}
