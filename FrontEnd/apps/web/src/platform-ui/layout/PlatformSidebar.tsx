/**
 * PlatformSidebar — generic, composable sidebar shell.
 *
 * Provides the structural regions (header / body / footer) of a sidebar. The
 * actual navigation is rendered by `PlatformNav` (driven by `usePlatformNav`),
 * keeping permission logic in the platform and markup in the theme.
 */

import type { ReactNode } from 'react';

interface PlatformSidebarProps {
  /** Top region (brand + toggler). */
  header?: ReactNode;
  /** Main scrollable region (typically `PlatformNav`). */
  children?: ReactNode;
  /** Bottom region (user card + sign-out). */
  footer?: ReactNode;
  /** Extra classes for the sidebar element. */
  className?: string;
}

export function PlatformSidebar({ header, children, footer, className }: PlatformSidebarProps) {
  return (
    <div className={['sidebar', 'sidebar-fixed', 'd-flex', 'flex-column', className].filter(Boolean).join(' ')}>
      {header !== undefined ? (
        <div className="sidebar-header border-bottom d-flex align-items-center justify-content-between px-3">
          {header}
        </div>
      ) : null}

      <div className="sidebar-body d-flex flex-column flex-grow-1 overflow-y-auto">
        {children}
      </div>

      {footer !== undefined ? (
        <div className="sidebar-footer border-top px-3 py-3 mt-auto">{footer}</div>
      ) : null}
    </div>
  );
}
