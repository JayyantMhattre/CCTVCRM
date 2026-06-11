/**
 * Navigation model vocabulary — the platform's source of truth for menus.
 *
 * Authoring types (`*Config`) carry visibility rules; resolved types
 * (`NavigationGroup`, `NavigationItem`) are the rule-free shapes the theme
 * renders (re-exported from the theme contract).
 */

export type { NavigationVisibility } from './NavigationVisibility';
export type { NavigationItemConfig, NavigationItem } from './NavigationItem';
export type { NavigationGroupConfig, NavigationGroup } from './NavigationGroup';
export type { NavigationDivider } from './NavigationDivider';
export type {
  NavigationBadge,
  NavigationBadgeVariant,
} from './NavigationBadge';
export type { NavigationIconKey } from '@/theme/contracts';
