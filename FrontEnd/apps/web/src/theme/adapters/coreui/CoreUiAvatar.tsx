/**
 * CoreUI adapter — avatar.
 *
 * Renders a circular avatar. When an image `src` is supplied it is shown;
 * otherwise initials derived from `name` are rendered on the brand colour —
 * the same treatment used today by the layout header and sidebar footer.
 */

import type { CSSProperties } from 'react';
import type { PlatformAvatarProps, PlatformAvatarSize } from '@/theme/contracts';

interface SizeSpec {
  readonly box: string;
  readonly font: string;
}

const SIZE_SPEC: Record<PlatformAvatarSize, SizeSpec> = {
  sm: { box: '2rem', font: '0.75rem' },
  md: { box: '2.5rem', font: '0.9rem' },
  lg: { box: '3.5rem', font: '1.25rem' },
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

export function CoreUiAvatar({ name, src, size = 'md', className }: PlatformAvatarProps) {
  const spec = SIZE_SPEC[size];
  const style: CSSProperties = { width: spec.box, height: spec.box };

  if (src) {
    return (
      <img
        src={src}
        alt={name}
        title={name}
        className={['rounded-circle', 'object-fit-cover', 'flex-shrink-0', className]
          .filter(Boolean)
          .join(' ')}
        style={style}
      />
    );
  }

  return (
    <span
      className={[
        'rounded-circle',
        'bg-primary',
        'text-white',
        'd-inline-flex',
        'align-items-center',
        'justify-content-center',
        'fw-semibold',
        'flex-shrink-0',
        className,
      ]
        .filter(Boolean)
        .join(' ')}
      style={{ ...style, fontSize: spec.font }}
      title={name}
      aria-label={name}
    >
      {deriveInitials(name)}
    </span>
  );
}
