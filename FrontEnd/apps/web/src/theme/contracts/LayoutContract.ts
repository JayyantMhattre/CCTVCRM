/**
 * LayoutContract — the application shell surfaces a theme must implement.
 *
 * Two shells exist:
 *   - `Layout`     — the authenticated application shell (sidebar + header +
 *                    content outlet + footer).
 *   - `AuthLayout` — the unauthenticated shell (centred auth card).
 *
 * The platform passes everything the shell needs as props (nav items, user,
 * dark-mode state, handlers). A theme decides the markup and styling only.
 */

import type { ComponentType, ReactNode } from 'react';
import type { AuthUser } from '@/shared/types/auth.types';
import type { NavigationGroup } from './NavigationContract';

/** Props for the authenticated application shell. */
export interface PlatformLayoutProps {
  /**
   * Fully-resolved navigation model (groups + items with `visible` flags),
   * produced by the platform. The theme renders it and makes no access
   * decisions. The full list — including not-visible groups — is provided so a
   * theme can preserve separator placement.
   */
  navGroups: readonly NavigationGroup[];
  /** The current user, or null while resolving. */
  user: AuthUser | null;
  /** Sign the current user out. */
  onLogout: () => void;
  /** Product display name. */
  appName: string;
  /**
   * Whether dark mode is active. Optional: a theme MAY manage its own
   * dark-mode mechanism internally (the CoreUI adapter does today), in which
   * case the platform does not supply these.
   */
  isDarkMode?: boolean;
  /** Toggle dark/light mode. Optional — see {@link PlatformLayoutProps.isDarkMode}. */
  onToggleDarkMode?: () => void;
  /**
   * Page content. When omitted, the theme is expected to render the router
   * `<Outlet />` itself. Provided for composability / testing.
   */
  children?: ReactNode;
}

/** Props for the unauthenticated (auth pages) shell. */
export interface PlatformAuthLayoutProps {
  /** Product display name. */
  appName: string;
  /**
   * Auth page content. When omitted, the theme renders the router `<Outlet />`.
   */
  children?: ReactNode;
}

/** The authenticated shell component. */
export type PlatformLayoutComponent = ComponentType<PlatformLayoutProps>;

/** The unauthenticated shell component. */
export type PlatformAuthLayoutComponent = ComponentType<PlatformAuthLayoutProps>;

/** The layout portion of a {@link ThemeAdapter}. */
export interface LayoutContract {
  Layout: PlatformLayoutComponent;
  AuthLayout: PlatformAuthLayoutComponent;
}
