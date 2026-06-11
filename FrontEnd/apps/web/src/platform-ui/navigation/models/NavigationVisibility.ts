/**
 * NavigationVisibility — declarative access rules for a navigation node.
 *
 * This is a PLATFORM-only concept: the theme never sees it. The platform
 * resolver (`usePlatformNav`) evaluates these rules against the current user and
 * produces a plain `visible` boolean for the theme to render.
 *
 * Semantics (chosen to match the existing guards exactly):
 *   - `roles`        — visible if the user holds AT LEAST ONE role (mirrors
 *                      `RoleGuard` / `useAuth().hasRole(...roles)`).
 *   - `permissions`  — visible only if the user holds ALL listed permissions
 *                      (mirrors `PermissionGuard` for the single-permission case).
 *   - `featureFlags` — visible only if ALL listed feature flags are enabled.
 *
 * An omitted/empty rule imposes no restriction.
 */
export interface NavigationVisibility {
  /** Visible if the user has at least one of these roles. */
  roles?: readonly string[];
  /** Visible only if the user has all of these permissions. */
  permissions?: readonly string[];
  /** Visible only if all of these feature flags are enabled. */
  featureFlags?: readonly string[];
}
