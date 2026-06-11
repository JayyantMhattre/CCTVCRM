/**
 * PlatformAvatar — theme-agnostic user avatar.
 *
 * Delegates to the active theme adapter's `Avatar`. The platform supplies a
 * display `name` (used for initials + accessible label) and an optional image
 * `src`; the theme decides the rendering.
 */

import { useTheme } from '@/theme';
import type { PlatformAvatarProps } from '@/theme';

export function PlatformAvatar(props: PlatformAvatarProps) {
  const { adapter } = useTheme();
  const Avatar = adapter.avatar.Avatar;
  return <Avatar {...props} />;
}
