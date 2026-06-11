/**
 * BadgeContract — the status/category pill surface a theme must implement.
 *
 * Modules express intent semantically (`variant="success"`) and never write a
 * vendor CSS class. The CoreUI adapter maps the variant to Bootstrap/CoreUI
 * `badge bg-*` classes; a future theme maps the same variant to its own tokens.
 */

import type { ComponentType, ReactNode } from 'react';

/** Contextual colour variants for a badge (semantic, theme-agnostic). */
export type PlatformBadgeVariant =
  | 'primary'
  | 'secondary'
  | 'success'
  | 'danger'
  | 'warning'
  | 'info'
  | 'light'
  | 'dark';

/** Props for a themed badge. */
export interface PlatformBadgeProps {
  /** Semantic colour variant. Defaults to `secondary`. */
  variant?: PlatformBadgeVariant;
  /** Render as a fully-rounded pill. */
  pill?: boolean;
  /** Badge content (text, count, icon). */
  children: ReactNode;
  /** Extra classes for the badge element. */
  className?: string;
}

/** A theme's badge component. */
export type PlatformBadgeComponent = ComponentType<PlatformBadgeProps>;

/** The badge portion of a {@link ThemeAdapter}. */
export interface BadgeContract {
  Badge: PlatformBadgeComponent;
}
