/**
 * Centralised API endpoint registry.
 *
 * All URL strings live here — modules never hardcode paths.
 * Changing the API version or a resource name requires editing only this file.
 *
 * Convention:
 *   ENDPOINTS.<module>.<action>(...params) → string
 *
 * The base prefix (`/api/v1`) is composed from the env vars so the same
 * object works across versions without any module-level change.
 */

const VERSION = import.meta.env.VITE_API_VERSION ?? 'v1';

/** Root prefixes */
const API   = `/api/${VERSION}`;   // versioned REST endpoints
const OAUTH = '/connect';          // OpenIddict token endpoint (not versioned)

export const ENDPOINTS = {
  // ── Auth (OpenIddict + registration) ─────────────────────────────────────
  auth: {
    /** POST — resource-owner password grant */
    token:    `${OAUTH}/token`,
    /** POST — new identity registration */
    register: `${API}/auth/register`,
    /** GET  — start Google SSO challenge */
    ssoGoogle:    `${API}/auth/sso/google`,
    /** GET  — start Microsoft SSO challenge */
    ssoMicrosoft: `${API}/auth/sso/microsoft`,
    /** GET  — SSO callback (returns external claims) */
    ssoCallback:  `${API}/auth/sso/callback`,
  },

  // ── Tenants ───────────────────────────────────────────────────────────────
  tenants: {
    /** GET  — tenant resolved from JWT context */
    current:        `${API}/tenants/current`,
    /** GET/PATCH — tenant workspace settings (contract; may return 404 until backend exposes) */
    settings:       `${API}/tenants/current/settings`,
    /** POST — provision a new tenant */
    provision:      `${API}/tenants`,
    /** GET  — tenant by explicit id */
    byId: (id: string) => `${API}/tenants/${id}`,
  },

  // ── Users ─────────────────────────────────────────────────────────────────
  users: {
    /** GET  — all users for the JWT-resolved tenant */
    currentTenant: `${API}/users/tenant/current`,
    /** GET  — all users for an explicit tenant */
    byTenant: (tenantId: string) => `${API}/users/tenant/${tenantId}`,
    /** GET  — user profile by id */
    byId:     (userId: string)   => `${API}/users/${userId}`,
    /** PATCH — user preferences (contract; may return 404 until backend exposes) */
    preferences: (userId: string) => `${API}/users/${userId}/preferences`,
    /** Shorthand for the signed-in user's preferences */
    myPreferences: `${API}/users/me/preferences`,
  },

  // ── Audit ─────────────────────────────────────────────────────────────────
  audit: {
    /** GET  — paginated audit log (admin only) */
    logs: `${API}/audit-logs`,
  },

  // ── API Keys ──────────────────────────────────────────────────────────────
  apikeys: {
    list: `${API}/api-keys`,
    byId: (id: string) => `${API}/api-keys/${id}`,
    rotate: (id: string) => `${API}/api-keys/${id}/rotate`,
    revoke: (id: string) => `${API}/api-keys/${id}/revoke`,
    scopes: (id: string) => `${API}/api-keys/${id}/scopes`,
  },

  // ── Webhooks ──────────────────────────────────────────────────────────────
  webhooks: {
    subscriptions: `${API}/webhooks/subscriptions`,
    subscription: (id: string) => `${API}/webhooks/subscriptions/${id}`,
    rotateSecret: (id: string) => `${API}/webhooks/subscriptions/${id}/rotate-secret`,
    disable: (id: string) => `${API}/webhooks/subscriptions/${id}/disable`,
    deliveries: `${API}/webhooks/deliveries`,
    delivery: (id: string) => `${API}/webhooks/deliveries/${id}`,
    retryDelivery: (id: string) => `${API}/webhooks/deliveries/${id}/retry`,
    deadLetters: `${API}/webhooks/deadletters`,
    deadLetter: (id: string) => `${API}/webhooks/deadletters/${id}`,
    replayDeadLetter: (id: string) => `${API}/webhooks/deadletters/${id}/replay`,
  },
} as const;
