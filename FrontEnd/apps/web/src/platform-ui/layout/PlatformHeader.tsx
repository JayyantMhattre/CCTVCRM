/**
 * PlatformHeader — generic, composable application header shell.
 *
 * A presentational building block theme layouts can compose. It defines the
 * structural regions (start / center / end) without prescribing brand-specific
 * content, so each theme fills the slots with its own markup.
 */

import type { ReactNode } from 'react';

interface PlatformHeaderProps {
  /** Leading content (e.g. menu toggler, brand). */
  start?: ReactNode;
  /** Center content (e.g. breadcrumb, page title). */
  children?: ReactNode;
  /** Trailing content (e.g. theme toggle, user menu). */
  end?: ReactNode;
  /** Extra classes for the header element. */
  className?: string;
}

export function PlatformHeader({ start, children, end, className }: PlatformHeaderProps) {
  return (
    <header className={['header', 'header-sticky', className].filter(Boolean).join(' ')}>
      <div className="container-fluid d-flex align-items-center">
        {start}
        {children}
        <div className="ms-auto d-flex align-items-center gap-3">{end}</div>
      </div>
    </header>
  );
}
