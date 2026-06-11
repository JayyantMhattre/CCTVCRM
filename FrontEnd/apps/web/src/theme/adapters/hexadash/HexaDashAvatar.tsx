/**
 * HexaDash adapter — avatar.
 *
 * Circular avatar showing an image when `src` is provided, otherwise initials
 * derived from `name` on the HexaDash brand colour.
 */

import type { PlatformAvatarProps, PlatformAvatarSize } from '@/theme/contracts';

const SIZE_CLASS: Record<PlatformAvatarSize, string> = {
  sm: 'hexadash-avatar-sm',
  md: 'hexadash-avatar-md',
  lg: 'hexadash-avatar-lg',
};

/** Derives up to two uppercase initials from a display name. */
function deriveInitials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  const first = parts.at(0);
  const last = parts.at(-1);

  if (!first || !last) {
    return '?';
  }

  const initials = first === last ? first.charAt(0) : first.charAt(0) + last.charAt(0);
  return initials.toUpperCase();
}

export function HexaDashAvatar({ name, src, size = 'md', className }: PlatformAvatarProps) {
  const classes = ['hexadash-avatar', SIZE_CLASS[size], className].filter(Boolean).join(' ');

  if (src) {
    return <img src={src} alt={name} title={name} className={classes} />;
  }

  return (
    <span className={classes} title={name} aria-label={name}>
      {deriveInitials(name)}
    </span>
  );
}
