/**
 * PlatformToast — theme-agnostic notification viewport.
 *
 * Renders the active theme adapter's notification `Viewport`. The toast queue
 * and service remain platform-owned (`shared/ui/toast`); only the visual
 * presentation is themed.
 */

import { useTheme } from '@/theme';

export function PlatformToast() {
  const { adapter } = useTheme();
  const Viewport = adapter.notification.Viewport;
  return <Viewport />;
}
