/**
 * TabsContract — the tabbed-content surface a theme must implement.
 *
 * The platform resolves the declarative `<PlatformTab>` children into a plain
 * `items` model and owns the active-tab state. The theme renders the tab strip
 * and the active panel only — it makes no state decisions.
 */

import type { ComponentType, ReactNode } from 'react';

/** A single resolved tab (id + label + panel content). */
export interface PlatformTabItem {
  /** Stable unique id (used as React key and active-state hint). */
  id: string;
  /** Tab strip label. */
  label: ReactNode;
  /** Panel content shown when this tab is active. */
  content: ReactNode;
  /** Whether the tab is selectable. */
  disabled?: boolean;
}

/** Props passed to a theme's tabs renderer. */
export interface PlatformTabsViewProps {
  /** Ordered tab definitions. */
  items: readonly PlatformTabItem[];
  /** Currently active tab id (resolved by the platform). */
  activeId: string;
  /** Called when the user selects a different tab. */
  onChange: (id: string) => void;
  /** Accessible label for the tablist. */
  ariaLabel?: string;
  /** Extra classes for the wrapper. */
  className?: string;
}

/** A theme's tabs renderer component. */
export type PlatformTabsViewComponent = ComponentType<PlatformTabsViewProps>;

/** The tabs portion of a {@link ThemeAdapter}. */
export interface TabsContract {
  Tabs: PlatformTabsViewComponent;
}
