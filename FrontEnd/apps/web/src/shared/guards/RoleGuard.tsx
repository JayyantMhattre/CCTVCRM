/**
 * RoleGuard — route-level guard that checks role membership.
 *
 * The user must hold AT LEAST ONE of the provided `roles` to access the
 * nested route.  If the check fails, they are redirected to `redirectTo`
 * (defaults to the /403 Forbidden page).
 *
 * Usage in router (wraps one or more child routes):
 *   {
 *     element: <RoleGuard roles={['Admin']} />,
 *     children: [
 *       { path: '/audit', element: <AuditLogPage /> }
 *     ]
 *   }
 *
 * Usage as a component (inline conditional rendering):
 *   <RoleGuard roles={['Admin', 'Manager']} inline>
 *     <DeleteButton />
 *   </RoleGuard>
 */

import type { ReactNode } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth }  from '@/shared/hooks/useAuth';
import { ROUTES }   from '@/core/router/routeMap';

interface RoleGuardProps {
  /** The user must have at least one of these roles. */
  roles: string[];
  /**
   * Where to redirect on failure when used as a route element.
   * Defaults to `/403`.
   */
  redirectTo?: string;
  /**
   * When `true`, renders `children` or `null` instead of redirecting.
   * Use this for inline role-based UI rendering (e.g. hiding a button).
   */
  inline?: boolean;
  /** Content to render when `inline={true}` and the check passes. */
  children?: ReactNode;
}

export function RoleGuard({
  roles,
  redirectTo = ROUTES.forbidden,
  inline     = false,
  children,
}: RoleGuardProps) {
  const { hasRole } = useAuth();
  const allowed = hasRole(...roles);

  // ── Route mode (default) ────────────────────────────────────────────────────
  if (!inline) {
    return allowed ? <Outlet /> : <Navigate to={redirectTo} replace />;
  }

  // ── Inline mode — hide/show children ────────────────────────────────────────
  if (!allowed) return null;
  return <>{children}</>;
}
