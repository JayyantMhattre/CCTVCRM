/**
 * PlatformPageHeader — consistent page title + subtitle + action slot.
 *
 * Theme-neutral structural component. Mirrors the existing
 * `shared/components/PageHeader` so module pages can migrate to platform-ui
 * without visual change in a later phase.
 */

import type { ReactNode } from 'react';

interface PlatformPageHeaderProps {
  title: string;
  subtitle?: string;
  /** Right-aligned actions (e.g. buttons). */
  children?: ReactNode;
}

export function PlatformPageHeader({ title, subtitle, children }: PlatformPageHeaderProps) {
  return (
    <div className="d-flex justify-content-between align-items-start mb-4 pb-3 border-bottom">
      <div>
        <h1 className="h3 mb-0 fw-bold text-body">{title}</h1>
        {subtitle ? <p className="text-body-secondary mb-0 mt-1 small">{subtitle}</p> : null}
      </div>
      {children ? <div className="d-flex gap-2 flex-shrink-0 ms-3">{children}</div> : null}
    </div>
  );
}
