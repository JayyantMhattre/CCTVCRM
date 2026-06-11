/**
 * Navigation item models.
 *
 *   - `NavigationItemConfig` — the AUTHORING shape: declares the item plus its
 *     optional visibility rules. Authored in `navigationConfig.ts`.
 *   - `NavigationItem` (re-exported) — the RESOLVED shape the theme renders, with
 *     a computed `visible` boolean and no rules.
 */

import type { NavigationIconKey, NavigationBadge } from '@/theme/contracts';
import type { NavigationVisibility } from './NavigationVisibility';

/** Authoring shape for a single navigation item. */
export interface NavigationItemConfig {
  /** Stable unique id. */
  id: string;
  /** Human-readable label. */
  label: string;
  /** Target route path (use `ROUTES.*`). */
  to: string;
  /** Optional theme-agnostic icon key. */
  icon?: NavigationIconKey;
  /** Optional badge. */
  badge?: NavigationBadge;
  /** Optional access rules; omit for an always-visible item. */
  visibility?: NavigationVisibility;
}

export type { NavigationItem } from '@/theme/contracts';
