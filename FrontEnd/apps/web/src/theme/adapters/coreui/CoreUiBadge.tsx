/**
 * CoreUI adapter — badge.
 *
 * Maps the platform's semantic `variant` to Bootstrap/CoreUI `badge bg-*`
 * classes, mirroring the inline badges used today (e.g. webhook status,
 * audit event type). Light/warning/info variants get dark text for contrast,
 * matching the existing `StatusBadge` treatment.
 */

import type { PlatformBadgeProps, PlatformBadgeVariant } from '@/theme/contracts';

/** Variants that need dark text on a light background for contrast. */
const DARK_TEXT_VARIANTS: ReadonlySet<PlatformBadgeVariant> = new Set([
  'warning',
  'info',
  'light',
]);

export function CoreUiBadge({
  variant = 'secondary',
  pill = false,
  children,
  className,
}: PlatformBadgeProps) {
  const classes = [
    'badge',
    `bg-${variant}`,
    DARK_TEXT_VARIANTS.has(variant) ? 'text-dark' : '',
    pill ? 'rounded-pill' : '',
    className,
  ]
    .filter(Boolean)
    .join(' ');

  return <span className={classes}>{children}</span>;
}
