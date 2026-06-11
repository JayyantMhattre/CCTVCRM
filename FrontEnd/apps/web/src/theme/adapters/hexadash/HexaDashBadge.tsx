/**
 * HexaDash adapter — badge.
 *
 * Renders a HexaDash-styled tag/badge. Maps the platform's semantic `variant`
 * to `.hexadash-badge-*` classes (styled by `hexadash.scss` from the extracted
 * tokens). No Ant Design `Tag` is used.
 */

import type { PlatformBadgeProps } from '@/theme/contracts';

export function HexaDashBadge({
  variant = 'secondary',
  pill = false,
  children,
  className,
}: PlatformBadgeProps) {
  const classes = [
    'hexadash-badge',
    `hexadash-badge-${variant}`,
    pill ? 'hexadash-badge-pill' : '',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  return <span className={classes}>{children}</span>;
}
