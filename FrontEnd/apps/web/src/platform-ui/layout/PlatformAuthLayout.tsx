/**
 * PlatformAuthLayout — theme-agnostic unauthenticated (auth pages) shell.
 *
 * Thin orchestration layer (T2): owns the platform auth gating (loading spinner
 * while the session restores; redirect to the dashboard when already
 * authenticated) and delegates the visual shell to the active theme adapter's
 * `AuthLayout`.
 *
 *   Router → PlatformAuthLayout → Theme Adapter → CoreUI AuthLayout
 *
 * The gating logic is identical to the pre-T2 `AuthLayout`.
 */

import { Navigate } from 'react-router-dom';
import { useTheme } from '@/theme';
import { useAuth } from '@/shared/hooks/useAuth';
import { Spinner } from '@/shared/components/Spinner';
import { ROUTES } from '@/core/router/routeMap';

export function PlatformAuthLayout() {
  const { adapter } = useTheme();
  const { status } = useAuth();

  // While the session is being restored, show a centred spinner so the user
  // doesn't see a flash of the login form.
  if (status === 'loading') return <Spinner fullPage />;

  // Already logged in — skip the auth pages entirely.
  if (status === 'authenticated') return <Navigate to={ROUTES.dashboard} replace />;

  const AuthLayout = adapter.layout.AuthLayout;

  return <AuthLayout appName={import.meta.env.VITE_APP_NAME ?? 'Ashraak'} />;
}
