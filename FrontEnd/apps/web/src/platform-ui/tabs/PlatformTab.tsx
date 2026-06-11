/**
 * PlatformTab — declarative descriptor for a single tab.
 *
 * It renders nothing on its own; `PlatformTabs` reads its props (id, label,
 * content) to build the model handed to the active theme adapter. This mirrors
 * the React Router `<Route>` descriptor pattern.
 */

import type { ReactNode } from 'react';

export interface PlatformTabProps {
  /** Stable unique id for this tab. */
  tabId: string;
  /** Tab strip label. */
  label: ReactNode;
  /** Whether the tab is selectable. */
  disabled?: boolean;
  /** Panel content shown when this tab is active. */
  children: ReactNode;
}

/** Descriptor only — `PlatformTabs` consumes the props; this never renders. */
export function PlatformTab(_props: PlatformTabProps): null {
  return null;
}
