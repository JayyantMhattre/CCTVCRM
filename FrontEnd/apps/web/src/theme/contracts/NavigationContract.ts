/**
 * NavigationContract — the navigation surface a theme must implement.
 *
 * Ownership (T3):
 *   - The PLATFORM owns *what* the navigation contains and *who* may see it.
 *     It resolves role / permission / feature-flag rules and produces a plain
 *     **render model** — groups and items already carrying a boolean `visible`
 *     flag. See `platform-ui/navigation`.
 *   - The THEME owns *how* the model is rendered (markup, classes, icon glyphs).
 *
 * The render model below carries **no** roles, permissions, routes-knowledge or
 * business rules — only resolved, presentational data. A theme adapter therefore
 * cannot (and must not) make any access decision; it renders what it is given.
 */

import type { ComponentType } from 'react';

/**
 * Stable, theme-agnostic icon identifier. The platform references icons by key;
 * each theme maps the key to its own icon implementation (the CoreUI adapter
 * maps these to `@coreui/icons` glyphs). Keeping this a string union — rather
 * than a concrete icon node — is what keeps the platform free of any specific
 * icon library.
 */
export type NavigationIconKey =
  | 'dashboard'
  | 'tenant'
  | 'tenant-settings'
  | 'users'
  | 'profile'
  | 'audit';

/** Bootstrap/CoreUI-style contextual colour variants for a badge. */
export type NavigationBadgeVariant =
  | 'primary'
  | 'secondary'
  | 'success'
  | 'danger'
  | 'warning'
  | 'info'
  | 'light'
  | 'dark';

/** Optional badge rendered alongside a navigation item (e.g. a count). */
export interface NavigationBadge {
  /** Badge text. */
  label: string;
  /** Optional colour variant. */
  variant?: NavigationBadgeVariant;
}

/**
 * A single, **resolved** navigation entry produced by the platform and rendered
 * by the active theme. `visible` is pre-computed by the platform.
 */
export interface NavigationItem {
  /** Stable unique id (used as React key and active-state hint). */
  id: string;
  /** Human-readable label. */
  label: string;
  /** Target route path (matches `ROUTES.*`). */
  to: string;
  /** Optional theme-agnostic icon key. */
  icon?: NavigationIconKey;
  /** Optional badge. */
  badge?: NavigationBadge;
  /** Whether the current user may see this item (resolved by the platform). */
  visible: boolean;
}

/**
 * A **resolved** navigation group (section). `visible` is pre-computed by the
 * platform. The full group list — including not-visible groups — is passed to
 * the theme so it can preserve separator placement; the theme renders content
 * only for visible groups and items.
 */
export interface NavigationGroup {
  /** Stable unique id. */
  id: string;
  /** Optional section heading. */
  title?: string;
  /** Whether the group is visible to the current user (resolved by the platform). */
  visible: boolean;
  /** Items belonging to this group. */
  items: readonly NavigationItem[];
}

/** Props passed to a theme's navigation renderer. */
export interface PlatformNavProps {
  /** The full, ordered list of resolved navigation groups. */
  groups: readonly NavigationGroup[];
  /** Accessible label for the surrounding nav landmark. */
  ariaLabel?: string;
}

/** A theme's navigation renderer component. */
export type PlatformNavComponent = ComponentType<PlatformNavProps>;

/** The navigation portion of a {@link ThemeAdapter}. */
export interface NavigationContract {
  Nav: PlatformNavComponent;
}
