/**
 * PageHeader — consistent page title + optional subtitle and action slot.
 *
 * Example:
 *   <PageHeader title="Users" subtitle="Manage tenant members">
 *     <Button variant="primary">Invite User</Button>
 *   </PageHeader>
 */

import type { ReactNode } from 'react';

interface PageHeaderProps {
  title:     string;
  subtitle?: string;
  /** Right-aligned action area (e.g. buttons). */
  children?: ReactNode;
}

export function PageHeader({ title, subtitle, children }: PageHeaderProps) {
  return (
    <div className="d-flex justify-content-between align-items-start mb-4 pb-3 border-bottom">
      <div>
        {/* CoreUI uses --cui-body-color for headings in both light and dark modes */}
        <h1 className="h3 mb-0 fw-bold text-body">{title}</h1>
        {subtitle && (
          <p className="text-body-secondary mb-0 mt-1 small">{subtitle}</p>
        )}
      </div>
      {children && <div className="d-flex gap-2 flex-shrink-0 ms-3">{children}</div>}
    </div>
  );
}
