/**
 * BreadcrumbContract — the breadcrumb-trail surface a theme must implement.
 *
 * The platform resolves the declarative `<PlatformBreadcrumbItem>` children into
 * a plain `items` model. Entries without a `to` target are treated as the
 * current page (rendered as active, non-interactive). The theme decides markup
 * and link rendering only.
 */

import type { ComponentType, ReactNode } from 'react';

/** A single resolved breadcrumb entry. */
export interface PlatformBreadcrumbEntry {
  /** Visible label. */
  label: ReactNode;
  /**
   * Link target. When omitted, the entry is the current page and is rendered
   * as active (non-interactive, `aria-current="page"`).
   */
  to?: string;
}

/** Props passed to a theme's breadcrumb renderer. */
export interface PlatformBreadcrumbViewProps {
  /** Ordered breadcrumb entries (root first, current page last). */
  items: readonly PlatformBreadcrumbEntry[];
  /** Accessible label for the nav landmark. Defaults to `Breadcrumb`. */
  ariaLabel?: string;
  /** Extra classes for the nav wrapper. */
  className?: string;
}

/** A theme's breadcrumb renderer component. */
export type PlatformBreadcrumbViewComponent = ComponentType<PlatformBreadcrumbViewProps>;

/** The breadcrumb portion of a {@link ThemeAdapter}. */
export interface BreadcrumbContract {
  Breadcrumb: PlatformBreadcrumbViewComponent;
}
