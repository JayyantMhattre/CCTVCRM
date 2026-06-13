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
  forgotPassword: '/forgot-password',
  resetPassword: '/reset-password',
  sessions: '/account/sessions',

  // ── Public website (anonymous) ─────────────────────────────────────────────
  public: {
    home: '/',
    about: '/about',
    services: '/services',
    amc: '/amc',
    contact: '/contact',
    gallery: '/gallery',
    testimonials: '/testimonials',
    getQuote: '/get-quote',
    amcInquiry: '/amc-inquiry',
  },

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

  // ── CCTV — Aarvii AMC (Sprint 0 placeholders) ─────────────────────────────
  cctv: {
    admin: {
      leads: '/admin/leads',
      leadDetail: '/admin/leads/:leadId',
      customers: '/admin/customers',
      customerDetail: '/admin/customers/:customerId',
      sites: '/admin/sites',
      siteDetail: '/admin/sites/:siteId',
      amcPlans: '/admin/amc/plans',
      amcPlanDetail: '/admin/amc/plans/:planId',
      amcContracts: '/admin/amc/contracts',
      amcContractDetail: '/admin/amc/contracts/:contractId',
      schedules: '/admin/schedules',
      visits: '/admin/visits',
      visitDetail: '/admin/visits/:visitId',
      tickets: '/admin/tickets',
      ticketDetail: '/admin/tickets/:ticketId',
      invoices: '/admin/invoices',
      invoiceCreate: '/admin/invoices/new',
      invoiceDetail: '/admin/invoices/:invoiceId',
      invoiceEdit: '/admin/invoices/:invoiceId/edit',
      engineers: '/admin/engineers',
      engineerCreate: '/admin/engineers/new',
      engineerDetail: '/admin/engineers/:engineerId',
      reports: '/admin/reports',
      reportView: '/admin/reports/:reportKey',
    },
    portal: {
      dashboard: '/portal/dashboard',
      amc: '/portal/amc',
      serviceHistory: '/portal/service/history',
      upcomingVisits: '/portal/service/upcoming',
      tickets: '/portal/tickets',
      ticketCreate: '/portal/tickets/new',
      ticketDetail: '/portal/tickets/:ticketId',
      invoices: '/portal/invoices',
      invoiceDetail: '/portal/invoices/:invoiceId',
      profile: '/portal/profile',
    },
    engineer: {
      dashboard: '/engineer',
      visits: '/engineer/visits',
      visitDetail: '/engineer/visits/:visitId',
      visitReport: '/engineer/visits/:visitId/report',
      tickets: '/engineer/tickets',
      ticketDetail: '/engineer/tickets/:ticketId',
      profile: '/engineer/profile',
    },
  },
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
