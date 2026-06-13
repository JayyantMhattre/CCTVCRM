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

  // ── CCTV — Aarvii AMC ─────────────────────────────────────────────────────
  cctv: {
    inquiries: `${API}/cctv/inquiries`,
    leads: {
      list: `${API}/cctv/leads`,
      byId: (id: string) => `${API}/cctv/leads/${id}`,
      status: (id: string) => `${API}/cctv/leads/${id}/status`,
      activities: (id: string) => `${API}/cctv/leads/${id}/activities`,
      remarks: (id: string) => `${API}/cctv/leads/${id}/remarks`,
      attachments: (id: string) => `${API}/cctv/leads/${id}/attachments`,
      convert: (id: string) => `${API}/cctv/leads/${id}/convert`,
    },
    customers: {
      list: `${API}/cctv/customers`,
      byId: (id: string) => `${API}/cctv/customers/${id}`,
      status: (id: string) => `${API}/cctv/customers/${id}/status`,
      sites: (id: string) => `${API}/cctv/customers/${id}/sites`,
    },
    sites: {
      list: `${API}/cctv/sites`,
      byId: (id: string) => `${API}/cctv/sites/${id}`,
      status: (id: string) => `${API}/cctv/sites/${id}/status`,
      contacts: (id: string) => `${API}/cctv/sites/${id}/contacts`,
      documents: (id: string) => `${API}/cctv/sites/${id}/documents`,
      document: (siteId: string, documentId: string) =>
        `${API}/cctv/sites/${siteId}/documents/${documentId}`,
      assetSummary: (id: string) => `${API}/cctv/sites/${id}/asset-summary`,
    },
    amcPlans: {
      list: `${API}/cctv/amc-plans`,
      byId: (id: string) => `${API}/cctv/amc-plans/${id}`,
      status: (id: string) => `${API}/cctv/amc-plans/${id}/status`,
      versions: (planId: string) => `${API}/cctv/amc-plans/${planId}/versions`,
      version: (planId: string, versionId: string) =>
        `${API}/cctv/amc-plans/${planId}/versions/${versionId}`,
      publishVersion: (planId: string, versionId: string) =>
        `${API}/cctv/amc-plans/${planId}/versions/${versionId}/publish`,
    },
    contracts: {
      list: `${API}/cctv/contracts`,
      byId: (id: string) => `${API}/cctv/contracts/${id}`,
      status: (id: string) => `${API}/cctv/contracts/${id}/status`,
      terms: (contractId: string) => `${API}/cctv/contracts/${contractId}/terms`,
      activateTerm: (contractId: string, termId: string) =>
        `${API}/cctv/contracts/${contractId}/terms/${termId}/activate`,
      documents: (contractId: string) => `${API}/cctv/contracts/${contractId}/documents`,
      renewalRequest: (contractId: string) => `${API}/cctv/contracts/${contractId}/renewal-request`,
    },
    renewalRequests: `${API}/cctv/renewal-requests`,
    schedules: {
      list: `${API}/cctv/schedules`,
      byId: (id: string) => `${API}/cctv/schedules/${id}`,
      assign: (id: string) => `${API}/cctv/schedules/${id}/assign`,
      reschedule: (id: string) => `${API}/cctv/schedules/${id}/reschedule`,
      cancel: (id: string) => `${API}/cctv/schedules/${id}/cancel`,
    },
    visits: {
      list: `${API}/cctv/visits`,
      byId: (id: string) => `${API}/cctv/visits/${id}`,
      approvals: `${API}/cctv/visits/approvals`,
      start: (id: string) => `${API}/cctv/visits/${id}/start`,
      remarks: (id: string) => `${API}/cctv/visits/${id}/remarks`,
      photos: (id: string) => `${API}/cctv/visits/${id}/photos`,
      selfie: (id: string) => `${API}/cctv/visits/${id}/selfie`,
      location: (id: string) => `${API}/cctv/visits/${id}/location`,
      signature: (id: string) => `${API}/cctv/visits/${id}/signature`,
      attachments: (id: string) => `${API}/cctv/visits/${id}/attachments`,
      submit: (id: string) => `${API}/cctv/visits/${id}/submit`,
      approve: (id: string) => `${API}/cctv/visits/${id}/approve`,
      return: (id: string) => `${API}/cctv/visits/${id}/return`,
    },
    tickets: {
      list: `${API}/cctv/tickets`,
      byId: (id: string) => `${API}/cctv/tickets/${id}`,
      assign: (id: string) => `${API}/cctv/tickets/${id}/assign`,
      status: (id: string) => `${API}/cctv/tickets/${id}/status`,
      comments: (id: string) => `${API}/cctv/tickets/${id}/comments`,
      attachments: (id: string) => `${API}/cctv/tickets/${id}/attachments`,
      close: (id: string) => `${API}/cctv/tickets/${id}/close`,
      reopen: (id: string) => `${API}/cctv/tickets/${id}/reopen`,
      portal: `${API}/cctv/portal/tickets`,
      engineer: `${API}/cctv/engineer/tickets`,
    },
    invoices: {
      list: `${API}/cctv/invoices`,
      byId: (id: string) => `${API}/cctv/invoices/${id}`,
      generate: (id: string) => `${API}/cctv/invoices/${id}/generate`,
      send: (id: string) => `${API}/cctv/invoices/${id}/send`,
      markPaid: (id: string) => `${API}/cctv/invoices/${id}/mark-paid`,
      cancel: (id: string) => `${API}/cctv/invoices/${id}/cancel`,
      pdf: (id: string) => `${API}/cctv/invoices/${id}/pdf`,
      portal: `${API}/cctv/portal/invoices`,
    },
    engineers: {
      list: `${API}/cctv/engineers`,
      byId: (id: string) => `${API}/cctv/engineers/${id}`,
      status: (id: string) => `${API}/cctv/engineers/${id}/status`,
      workload: (id: string) => `${API}/cctv/engineers/${id}/workload`,
    },
    engineer: {
      dashboard: `${API}/cctv/engineer/dashboard`,
      schedules: `${API}/cctv/engineer/schedules`,
      schedulesToday: `${API}/cctv/engineer/schedules/today`,
      visitById: (id: string) => `${API}/cctv/engineer/visits/${id}`,
      sync: `${API}/cctv/engineer/visits/sync`,
      tickets: `${API}/cctv/engineer/tickets`,
    },
    reports: {
      leads: `${API}/cctv/reports/leads`,
      customers: `${API}/cctv/reports/customers`,
      amc: `${API}/cctv/reports/amc`,
      visits: `${API}/cctv/reports/visits`,
      engineers: `${API}/cctv/reports/engineers`,
      tickets: `${API}/cctv/reports/tickets`,
      invoices: `${API}/cctv/reports/invoices`,
      revenue: `${API}/cctv/reports/revenue`,
      adminDashboard: `${API}/cctv/admin/dashboard`,
    },
    portalVisits: {
      upcoming: `${API}/cctv/portal/visits/upcoming`,
      history: `${API}/cctv/portal/visits/history`,
      byId: (id: string) => `${API}/cctv/portal/visits/${id}`,
    },
    portal: {
      profile: `${API}/cctv/portal/profile`,
      sites: `${API}/cctv/portal/sites`,
      siteDetail: (id: string) => `${API}/cctv/portal/sites/${id}`,
      amc: `${API}/cctv/portal/amc`,
      amcDocuments: `${API}/cctv/portal/amc/documents`,
    },
  },
} as const;
