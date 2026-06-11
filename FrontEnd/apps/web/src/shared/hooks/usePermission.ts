/**
 * usePermission — fine-grained permission check hook.
 *
 * Reads the current user's permission list from the auth store and exposes
 * a typed `hasPermission` function for use in components and guards.
 *
 * Permissions are string identifiers that match what the backend encodes
 * into the JWT (e.g. "audit:read", "users:manage", "tenant:configure").
 *
 * Example:
 *   const { hasPermission } = usePermission();
 *   if (hasPermission('audit:read')) { ... }
 */

import { useAuthStore } from '@/core/auth/authStore';

export function usePermission() {
  const user = useAuthStore((s) => s.user);

  /**
   * Returns true if the authenticated user has the specified permission.
   * Always returns false when no user is loaded.
   */
  function hasPermission(permission: string): boolean {
    return user?.permissions.includes(permission) ?? false;
  }

  /**
   * Returns true if the user has ALL of the provided permissions.
   */
  function hasAllPermissions(...permissions: string[]): boolean {
    return permissions.every((p) => hasPermission(p));
  }

  /**
   * Returns true if the user has ANY of the provided permissions.
   */
  function hasAnyPermission(...permissions: string[]): boolean {
    return permissions.some((p) => hasPermission(p));
  }

  return { hasPermission, hasAllPermissions, hasAnyPermission };
}
