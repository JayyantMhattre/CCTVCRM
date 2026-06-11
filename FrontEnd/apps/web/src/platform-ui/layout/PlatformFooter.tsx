/**
 * PlatformFooter — generic, composable application footer shell.
 */

import type { ReactNode } from 'react';

interface PlatformFooterProps {
  /** Left-aligned content (e.g. copyright). */
  children?: ReactNode;
  /** Right-aligned content (e.g. attribution / links). */
  end?: ReactNode;
  /** Extra classes for the footer element. */
  className?: string;
}

export function PlatformFooter({ children, end, className }: PlatformFooterProps) {
  return (
    <footer
      className={['footer', 'px-4', 'py-2', 'd-flex', 'align-items-center', 'justify-content-between', className]
        .filter(Boolean)
        .join(' ')}
    >
      <span className="small text-muted">{children}</span>
      {end !== undefined ? <span className="small text-muted d-none d-sm-inline">{end}</span> : null}
    </footer>
  );
}
