/**
 * Navigation group models.
 *
 *   - `NavigationGroupConfig` — the AUTHORING shape: a section plus its optional
 *     visibility rules and authored items.
 *   - `NavigationGroup` (re-exported) — the RESOLVED shape the theme renders.
 */

import type { NavigationItemConfig } from './NavigationItem';
import type { NavigationVisibility } from './NavigationVisibility';

/** Authoring shape for a navigation group (section). */
export interface NavigationGroupConfig {
  /** Stable unique id. */
  id: string;
  /** Optional section heading. */
  title?: string;
  /** Optional access rules applied to the whole group; omit for always-visible. */
  visibility?: NavigationVisibility;
  /** Items belonging to this group. */
  items: readonly NavigationItemConfig[];
}

export type { NavigationGroup } from '@/theme/contracts';
