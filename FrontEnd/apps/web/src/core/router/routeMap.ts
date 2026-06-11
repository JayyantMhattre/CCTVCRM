/**
 * Route name registry — the single source of truth for all route paths.
 *
 * Rules:
 *  - Use ROUTES.<section>.<page> constants everywhere; never raw strings.
 *  - Changing a URL means editing this file only.
 *  - Dynamic segments use `:param` syntax matching React Router convention.
 *
 * Adding a new module:
 *  1. Add its paths here under a new key.
 *  2. Add a lazy import + route object in `index.tsx`.
 *  3. Add a nav link in `AppLayout.tsx`.
 */

export const ROUTES = {
  // ── Public / auth ──────────────────────────────────────────────────────────
  login:    '/login',
  register: '/register',
  sessions: '/account/sessions',

  // ── Authenticated shell ────────────────────────────────────────────────────
  root:      '/',

  // ── Dashboard ─────────────────────────────────────────────────────────────
  dashboard: '/dashboard',

  // ── Tenant ────────────────────────────────────────────────────────────────
  tenant: {
    profile:  '/tenant/profile',
    settings: '/tenant/settings',
  },

  // ── Users ─────────────────────────────────────────────────────────────────
  users: {
    list:        '/users',
    profile:     '/users/:userId',
    preferences: '/users/me/notifications',
  },

  // ── Audit ─────────────────────────────────────────────────────────────────
  audit: {
    logs: '/audit',
  },

  // ── API Keys ──────────────────────────────────────────────────────────────
  apikeys: {
    list: '/api-keys',
    detail: '/api-keys/:id',
  },

  // ── Webhooks ──────────────────────────────────────────────────────────────
  webhooks: {
    overview: '/webhooks',
    subscriptions: '/webhooks/subscriptions',
    subscriptionDetail: '/webhooks/subscriptions/:id',
    deliveries: '/webhooks/deliveries',
    deliveryDetail: '/webhooks/deliveries/:id',
    deadLetters: '/webhooks/deadletters',
    deadLetterDetail: '/webhooks/deadletters/:id',
  },

  // ── Error pages ───────────────────────────────────────────────────────────
  forbidden:  '/403',
  notFound:   '/404',
} as const;

/** Resolved profile URL for a user id (use instead of raw `/users/:userId`). */
export function userProfileRoute(userId: string): string {
  return `/users/${userId}`;
}

export function apiKeyDetailRoute(id: string): string {
  return `/api-keys/${id}`;
}

export function webhookSubscriptionRoute(id: string): string {
  return `/webhooks/subscriptions/${id}`;
}

export function webhookDeliveryRoute(id: string): string {
  return `/webhooks/deliveries/${id}`;
}

export function webhookDeadLetterRoute(id: string): string {
  return `/webhooks/deadletters/${id}`;
}
