/**
 * AvatarContract — the user avatar surface a theme must implement.
 *
 * The platform supplies a display name (used to derive initials and the
 * accessible label) and an optional image source. The theme decides how the
 * avatar is rendered — the CoreUI adapter reuses the existing `.avatar-*`
 * styling used today by the layout header and user list.
 */

import type { ComponentType } from 'react';

/** Avatar size presets. */
export type PlatformAvatarSize = 'sm' | 'md' | 'lg';

/** Props for a themed avatar. */
export interface PlatformAvatarProps {
  /** Full display name — drives initials and the accessible label. */
  name: string;
  /** Optional image URL; when absent the avatar falls back to initials. */
  src?: string | null;
  /** Size preset. Defaults to `md`. */
  size?: PlatformAvatarSize;
  /** Extra classes for the avatar element. */
  className?: string;
}

/** A theme's avatar component. */
export type PlatformAvatarComponent = ComponentType<PlatformAvatarProps>;

/** The avatar portion of a {@link ThemeAdapter}. */
export interface AvatarContract {
  Avatar: PlatformAvatarComponent;
}
