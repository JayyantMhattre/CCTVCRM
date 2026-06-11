/**
 * PermissionGuard — inline component-level guard for fine-grained ABAC.
 *
 * Unlike RoleGuard, this guard is designed for component-level hiding only
 * (not route-level redirects).  It renders its children only when the
 * authenticated user has the required permission.
 *
 * Use cases:
 *  - Hiding an "Export CSV" button unless the user has `audit:export`.
 *  - Showing an "Invite User" link only with `users:invite` permission.
 *  - Disabling a "Delete Tenant" button without `tenant:delete`.
 *
 * Usage:
 *   <PermissionGuard permission="audit:read">
 *     <Link to="/audit">Audit Logs</Link>
 *   </PermissionGuard>
 *
 *   <PermissionGuard permission="users:manage" fallback={<ReadOnlyBadge />}>
 *     <EditUserButton />
 *   </PermissionGuard>
 */

import type { ReactNode } from 'react';
import { usePermission } from '@/shared/hooks/usePermission';

interface PermissionGuardProps {
  /** The specific permission string required. */
  permission: string;
  /**
   * Content to render if the user does NOT have the permission.
   * Defaults to `null` (renders nothing).
   */
  fallback?: ReactNode;
  children: ReactNode;
}

export function PermissionGuard({
  permission,
  fallback = null,
  children,
}: PermissionGuardProps) {
  const { hasPermission } = usePermission();

  return hasPermission(permission) ? <>{children}</> : <>{fallback}</>;
}
