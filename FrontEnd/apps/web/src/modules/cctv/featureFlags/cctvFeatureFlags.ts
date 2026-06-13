/**
 * CCTV feature flag keys — must stay in sync with
 * `Ashraak.SharedKernel.Contracts.CctvCrm.CctvFeatureFlags` (backend).
 *
 * Defaults are placeholders until modules ship (B1–B7). Override via
 * `VITE_CCTV_FEATURE_*` env vars or future tenant flag API.
 */
export const CCTV_FEATURE_FLAGS = {
  enabled: 'cctv.enabled',
  leads: 'cctv.leads.enabled',
  customers: 'cctv.customers.enabled',
  amc: 'cctv.amc.enabled',
  service: 'cctv.service.enabled',
  tickets: 'cctv.tickets.enabled',
  engineers: 'cctv.engineers.enabled',
  invoices: 'cctv.invoices.enabled',
  reporting: 'cctv.reporting.enabled',
  customerPortal: 'cctv.portal.customer.enabled',
  engineerPortal: 'cctv.portal.engineer.enabled',
  customerMobile: 'cctv.mobile.customer.enabled',
  engineerMobile: 'cctv.mobile.engineer.enabled',
  sms: 'cctv.integrations.sms.enabled',
  pdf: 'cctv.integrations.pdf.enabled',
} as const;

export type CctvFeatureFlagKey = (typeof CCTV_FEATURE_FLAGS)[keyof typeof CCTV_FEATURE_FLAGS];

/** Sprint 0 placeholder defaults — master on; module flags off until phase ships. */
export const CCTV_FEATURE_FLAG_DEFAULTS: Readonly<Record<CctvFeatureFlagKey, boolean>> = {
  [CCTV_FEATURE_FLAGS.enabled]: true,
  [CCTV_FEATURE_FLAGS.leads]: true,
  [CCTV_FEATURE_FLAGS.customers]: true,
  [CCTV_FEATURE_FLAGS.amc]: true,
  [CCTV_FEATURE_FLAGS.service]: true,
  [CCTV_FEATURE_FLAGS.tickets]: true,
  [CCTV_FEATURE_FLAGS.engineers]: true,
  [CCTV_FEATURE_FLAGS.invoices]: true,
  [CCTV_FEATURE_FLAGS.reporting]: true,
  [CCTV_FEATURE_FLAGS.customerPortal]: true,
  [CCTV_FEATURE_FLAGS.engineerPortal]: true,
  [CCTV_FEATURE_FLAGS.customerMobile]: true,
  [CCTV_FEATURE_FLAGS.engineerMobile]: true,
  [CCTV_FEATURE_FLAGS.sms]: false,
  [CCTV_FEATURE_FLAGS.pdf]: false,
};
