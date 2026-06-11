/**
 * AuthGuard — route-level guard that protects authenticated pages.
 *
 * Behaviour:
 *  - While session is being restored → show a full-page spinner.
 *  - If unauthenticated            → redirect to /login (preserving intended URL).
 *  - If authenticated              → render <Outlet /> (nested routes).
 *
 * Usage in router:
 *   { element: <AuthGuard />, children: [ ...protectedRoutes ] }
 *
 * The `state.from` on the redirect allows LoginPage to navigate the user
 * back to the page they originally tried to visit after a successful login.
 */

import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth }   from '@/shared/hooks/useAuth';
import { Spinner }   from '@/shared/components/Spinner';
import { ROUTES }    from '@/core/router/routeMap';

export function AuthGuard() {
  const { status } = useAuth();
  const location   = useLocation();

  // Restore in progress — do not flash the login page.
  if (status === 'loading') {
    return <Spinner fullPage />;
  }

  // Not logged in — preserve the intended destination.
  if (status === 'unauthenticated') {
    return (
      <Navigate
        to={ROUTES.login}
        state={{ from: location }}
        replace
      />
    );
  }

  // Authenticated — render the nested route.
  return <Outlet />;
}
