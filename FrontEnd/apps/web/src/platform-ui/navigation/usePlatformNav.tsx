/**
 * usePlatformNav — the single source of truth for navigation.
 *
 * Resolves the declarative `NAVIGATION_CONFIG` against the current user, applying
 * (in order) role, permission and feature-flag rules, and returns a plain render
 * model (`NavigationGroup[]`) for the active theme to render.
 *
 * Ownership boundary (T3):
 *   - ALL access decisions happen here, in the platform.
 *   - The theme receives only resolved `visible` booleans — never roles,
 *     permissions, flags or business rules.
 *
 * The full group list is returned (including not-visible groups) so the theme can
 * preserve separator placement identically to the pre-T3 sidebar.
 *
 * Visibility semantics match the existing guards exactly:
 *   - roles        → `hasRole(...roles)`            (at least one)
 *   - permissions  → every permission via `hasPermission` (all)
 *   - featureFlags → every flag via `isEnabled`     (all)
 */

import { useAuth } from '@/shared/hooks/useAuth';
import { usePermission } from '@/shared/hooks/usePermission';
import { NAVIGATION_CONFIG } from './navigationConfig';
import { useFeatureFlags } from './useFeatureFlags';
import type { NavigationGroup, NavigationVisibility } from './models';

export function usePlatformNav(): NavigationGroup[] {
  const { hasRole } = useAuth();
  const { hasPermission } = usePermission();
  const { isEnabled } = useFeatureFlags();

  function isVisible(rules: NavigationVisibility | undefined): boolean {
    if (!rules) {
      return true;
    }
    if (rules.roles && rules.roles.length > 0 && !hasRole(...rules.roles)) {
      return false;
    }
    if (
      rules.permissions &&
      rules.permissions.length > 0 &&
      !rules.permissions.every((permission) => hasPermission(permission))
    ) {
      return false;
    }
    if (
      rules.featureFlags &&
      rules.featureFlags.length > 0 &&
      !rules.featureFlags.every((flag) => isEnabled(flag))
    ) {
      return false;
    }
    return true;
  }

  return NAVIGATION_CONFIG.map((group) => {
    const groupVisible = isVisible(group.visibility);

    return {
      id: group.id,
      title: group.title,
      visible: groupVisible,
      items: group.items.map((item) => ({
        id: item.id,
        label: item.label,
        to: item.to,
        icon: item.icon,
        badge: item.badge,
        visible: groupVisible && isVisible(item.visibility),
      })),
    };
  });
}
