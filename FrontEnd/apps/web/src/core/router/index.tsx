/**
 * Root router configuration.
 *
 * Pattern: module routes are lazy-loaded (React.lazy + Suspense) to keep the
 * initial bundle small.  Each module owns its `routes.tsx` which returns a
 * React Router `RouteObject[]`.  Those are spread here inside the appropriate
 * guard wrapper.
 *
 * Route tree:
 *
 *   /login              → LoginPage          (public)
 *   /register           → RegisterPage       (public)
 *   /                   → AuthGuard
 *     /                   → AppLayout
 *       /dashboard        → DashboardPage
 *       /tenant/*         → TenantRoutes
 *       /users/*          → UserRoutes     [Role: Admin | Manager]
 *       /audit            → AuditLogPage   [Role: Admin]
 *       /webhooks/*       → Webhooks ops   [webhooks:read]
 *   /403                → ForbiddenPage
 *   *                   → NotFoundPage
 *
 * Adding a new module = one `...moduleRoutes` spread below.
 */

import { lazy, Suspense } from 'react';
import {
  createBrowserRouter,
  RouterProvider,
  Navigate,
  type RouteObject,
} from 'react-router-dom';

import { ROUTES }     from './routeMap';
import { AuthGuard }  from '@/shared/guards/AuthGuard';
import { RoleGuard }  from '@/shared/guards/RoleGuard';
import { ApiKeysRouteGuard } from '@/modules/apikeys/guards/ApiKeysRouteGuard';
import { WebhooksRouteGuard } from '@/modules/webhooks/guards/WebhooksRouteGuard';
import { PlatformLayout, PlatformAuthLayout } from '@/platform-ui';
import { Spinner }    from '@/shared/components/Spinner';

// ── Lazy page imports ─────────────────────────────────────────────────────────

const LoginPage     = lazy(() => import('@/modules/auth/pages/LoginPage'));
const RegisterPage  = lazy(() => import('@/modules/auth/pages/RegisterPage'));
const SessionsPage  = lazy(() => import('@/modules/auth/pages/SessionsPage'));
const DashboardPage = lazy(() => import('@/modules/dashboard/pages/DashboardPage'));

const TenantProfilePage  = lazy(() => import('@/modules/tenant/pages/TenantProfilePage'));
const TenantSettingsPage = lazy(() => import('@/modules/tenant/pages/TenantSettingsPage'));

const UserListPage    = lazy(() => import('@/modules/users/pages/UserListPage'));
const UserProfilePage = lazy(() => import('@/modules/users/pages/UserProfilePage'));
const NotificationPreferencesPage = lazy(
  () => import('@/modules/users/pages/NotificationPreferencesPage'),
);

const AuditLogPage = lazy(() => import('@/modules/audit/pages/AuditLogPage'));

const ApiKeysListPage = lazy(() => import('@/modules/apikeys/pages/ApiKeysListPage'));
const ApiKeyDetailPage = lazy(() => import('@/modules/apikeys/pages/ApiKeyDetailPage'));

const WebhooksOverviewPage = lazy(() => import('@/modules/webhooks/pages/WebhooksOverviewPage'));
const SubscriptionsPage = lazy(() => import('@/modules/webhooks/pages/SubscriptionsPage'));
const SubscriptionDetailPage = lazy(() => import('@/modules/webhooks/pages/SubscriptionDetailPage'));
const DeliveriesPage = lazy(() => import('@/modules/webhooks/pages/DeliveriesPage'));
const DeliveryDetailPage = lazy(() => import('@/modules/webhooks/pages/DeliveryDetailPage'));
const DeadLettersPage = lazy(() => import('@/modules/webhooks/pages/DeadLettersPage'));
const DeadLetterDetailPage = lazy(() => import('@/modules/webhooks/pages/DeadLetterDetailPage'));

const ForbiddenPage = lazy(() => import('@/shared/pages/ForbiddenPage'));
const NotFoundPage  = lazy(() => import('@/shared/pages/NotFoundPage'));

// ── Suspense wrapper ──────────────────────────────────────────────────────────

/** Wraps lazy pages in a Suspense boundary with a centred spinner. */
function Page({ children }: { children: React.ReactNode }) {
  return (
    <Suspense fallback={<Spinner fullPage />}>
      {children}
    </Suspense>
  );
}

// ── Route definitions ─────────────────────────────────────────────────────────

const routes: RouteObject[] = [
  // ── Public auth routes ─────────────────────────────────────────────────────
  {
    element: <PlatformAuthLayout />,
    children: [
      {
        path: ROUTES.login,
        element: <Page><LoginPage /></Page>,
      },
      {
        path: ROUTES.register,
        element: <Page><RegisterPage /></Page>,
      },
    ],
  },

  // ── Authenticated routes (protected by AuthGuard) ─────────────────────────
  {
    element: <AuthGuard />,
    children: [
      {
        element: <PlatformLayout />,
        children: [
          // Redirect / → /dashboard
          {
            path: ROUTES.root,
            element: <Navigate to={ROUTES.dashboard} replace />,
          },

          // Dashboard — any authenticated user
          {
            path: ROUTES.dashboard,
            element: <Page><DashboardPage /></Page>,
          },

          // Tenant — any authenticated user
          {
            path: ROUTES.tenant.profile,
            element: <Page><TenantProfilePage /></Page>,
          },
          {
            path: ROUTES.tenant.settings,
            element: <Page><TenantSettingsPage /></Page>,
          },

          // Notification preferences — any authenticated user
          {
            path: ROUTES.users.preferences,
            element: <Page><NotificationPreferencesPage /></Page>,
          },
          {
            path: ROUTES.sessions,
            element: <Page><SessionsPage /></Page>,
          },

          // Users — Admin or Manager only
          {
            element: <RoleGuard roles={['Admin', 'Manager']} />,
            children: [
              {
                path: ROUTES.users.list,
                element: <Page><UserListPage /></Page>,
              },
              {
                path: ROUTES.users.profile,
                element: <Page><UserProfilePage /></Page>,
              },
            ],
          },

          // Audit — Admin only
          {
            element: <RoleGuard roles={['Admin']} />,
            children: [
              {
                path: ROUTES.audit.logs,
                element: <Page><AuditLogPage /></Page>,
              },
            ],
          },

          // API Keys — apikeys:read or apikeys:manage
          {
            element: <ApiKeysRouteGuard />,
            children: [
              {
                path: ROUTES.apikeys.list,
                element: <Page><ApiKeysListPage /></Page>,
              },
              {
                path: ROUTES.apikeys.detail,
                element: <Page><ApiKeyDetailPage /></Page>,
              },
            ],
          },

          // Webhooks — webhooks:read or webhooks:manage
          {
            element: <WebhooksRouteGuard />,
            children: [
              {
                path: ROUTES.webhooks.overview,
                element: <Page><WebhooksOverviewPage /></Page>,
              },
              {
                path: ROUTES.webhooks.subscriptions,
                element: <Page><SubscriptionsPage /></Page>,
              },
              {
                path: ROUTES.webhooks.subscriptionDetail,
                element: <Page><SubscriptionDetailPage /></Page>,
              },
              {
                path: ROUTES.webhooks.deliveries,
                element: <Page><DeliveriesPage /></Page>,
              },
              {
                path: ROUTES.webhooks.deliveryDetail,
                element: <Page><DeliveryDetailPage /></Page>,
              },
              {
                path: ROUTES.webhooks.deadLetters,
                element: <Page><DeadLettersPage /></Page>,
              },
              {
                path: ROUTES.webhooks.deadLetterDetail,
                element: <Page><DeadLetterDetailPage /></Page>,
              },
            ],
          },
        ],
      },
    ],
  },

  // ── Error pages ────────────────────────────────────────────────────────────
  {
    path: ROUTES.forbidden,
    element: <Page><ForbiddenPage /></Page>,
  },
  {
    path: '*',
    element: <Page><NotFoundPage /></Page>,
  },
];

const router = createBrowserRouter(routes);

/** Drop-in RouterProvider — render this inside AppProviders. */
export function AppRouter() {
  return <RouterProvider router={router} />;
}
