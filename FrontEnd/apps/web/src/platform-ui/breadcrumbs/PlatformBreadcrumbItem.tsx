/**
 * PlatformBreadcrumbItem — declarative descriptor for one breadcrumb entry.
 *
 * Renders nothing on its own; `PlatformBreadcrumb` reads its props to build the
 * model handed to the active theme adapter. An item without a `to` target is
 * treated as the current page (rendered active by the theme).
 */

import type { ReactNode } from 'react';

export interface PlatformBreadcrumbItemProps {
  /** Visible label. */
  label: ReactNode;
  /** Link target; omit for the current page. */
  to?: string;
}

/** Descriptor only — `PlatformBreadcrumb` consumes the props; never renders. */
export function PlatformBreadcrumbItem(_props: PlatformBreadcrumbItemProps): null {
  return null;
}
