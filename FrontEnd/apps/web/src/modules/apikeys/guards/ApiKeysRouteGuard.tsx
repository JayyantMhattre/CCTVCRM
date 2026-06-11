import { Navigate, Outlet } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { useApiKeyPermissions } from '../hooks/useApiKeyPermissions';

/** Route guard — requires apikeys:read or apikeys:manage. */
export function ApiKeysRouteGuard() {
  const { canRead } = useApiKeyPermissions();
  return canRead ? <Outlet /> : <Navigate to={ROUTES.forbidden} replace />;
}
