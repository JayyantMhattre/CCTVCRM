/**
 * NavigationDivider — an explicit separator node.
 *
 * Today the active theme draws a separator between adjacent groups
 * automatically, so an explicit divider is not required by the current menu.
 * This type is part of the navigation vocabulary and is reserved for future
 * explicit, in-group separators (and alternative themes that need them).
 */
export interface NavigationDivider {
  /** Stable unique id. */
  id: string;
  /** Discriminator for use in mixed model arrays. */
  kind: 'divider';
}
