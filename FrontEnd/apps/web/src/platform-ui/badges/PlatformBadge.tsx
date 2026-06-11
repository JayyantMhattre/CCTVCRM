/**
 * PlatformBadge — theme-agnostic status/category pill.
 *
 * Delegates to the active theme adapter's `Badge`. Modules express intent with
 * a semantic `variant` (e.g. `success`, `danger`) and never write a vendor CSS
 * class.
 */

import { useTheme } from '@/theme';
import type { PlatformBadgeProps } from '@/theme';

export function PlatformBadge(props: PlatformBadgeProps) {
  const { adapter } = useTheme();
  const Badge = adapter.badge.Badge;
  return <Badge {...props} />;
}
