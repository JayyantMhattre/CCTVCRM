import { Navigate, Outlet } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { useAuth } from '@/shared/hooks/useAuth';

/** Route guard for CCTV admin placeholders — requires Admin role (Sprint 0). */
export function CctvAdminRouteGuard() {
  const { hasRole } = useAuth();
  return hasRole('Admin') ? <Outlet /> : <Navigate to={ROUTES.forbidden} replace />;
}
