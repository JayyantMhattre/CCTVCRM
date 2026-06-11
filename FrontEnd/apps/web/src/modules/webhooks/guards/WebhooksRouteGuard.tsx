import { Navigate, Outlet } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { useWebhookPermissions } from '../hooks/useWebhookPermissions';

/** Route guard — requires webhooks:read or webhooks:manage. */
export function WebhooksRouteGuard() {
  const { canRead } = useWebhookPermissions();
  return canRead ? <Outlet /> : <Navigate to={ROUTES.forbidden} replace />;
}
