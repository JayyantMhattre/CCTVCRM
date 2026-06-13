/**

 * CCTV portal navigation — customer self-service (D1-13b).

 */

import { ROUTES } from '@/core/router/routeMap';

import type { NavigationGroupConfig } from '@/platform-ui/navigation/models';

import { CCTV_FEATURE_FLAGS } from '../featureFlags/cctvFeatureFlags';



export const CCTV_ADMIN_NAV: readonly NavigationGroupConfig[] = [

  {

    id: 'cctv-leads',

    title: 'Leads',

    visibility: { permissions: ['leads:read'], featureFlags: [CCTV_FEATURE_FLAGS.leads] },

    items: [

      { id: 'cctv-leads-list', label: 'Lead Pipeline', to: ROUTES.cctv.admin.leads, icon: 'leads' },

    ],

  },

  {

    id: 'cctv-customers',

    title: 'Customers',

    visibility: { permissions: ['customers:read'], featureFlags: [CCTV_FEATURE_FLAGS.customers] },

    items: [

      { id: 'cctv-customers-list', label: 'Customers', to: ROUTES.cctv.admin.customers, icon: 'customers' },

      { id: 'cctv-sites-list', label: 'Sites', to: ROUTES.cctv.admin.sites, icon: 'sites' },

    ],

  },

  {

    id: 'cctv-amc',

    title: 'AMC',

    visibility: { permissions: ['amc:read'], featureFlags: [CCTV_FEATURE_FLAGS.amc] },

    items: [

      { id: 'cctv-amc-plans', label: 'AMC Plans', to: ROUTES.cctv.admin.amcPlans, icon: 'amc' },

      { id: 'cctv-amc-contracts', label: 'Contracts', to: ROUTES.cctv.admin.amcContracts, icon: 'contracts' },

    ],

  },

  {

    id: 'cctv-operations',

    title: 'Operations',

    visibility: { permissions: ['visits:read'], featureFlags: [CCTV_FEATURE_FLAGS.service, CCTV_FEATURE_FLAGS.tickets] },

    items: [

      { id: 'cctv-schedules', label: 'Schedules', to: ROUTES.cctv.admin.schedules, icon: 'schedule' },

      { id: 'cctv-visits', label: 'Visits', to: ROUTES.cctv.admin.visits, icon: 'visits' },

      { id: 'cctv-tickets', label: 'Tickets', to: ROUTES.cctv.admin.tickets, icon: 'tickets' },

    ],

  },

  {

    id: 'cctv-engineers',

    title: 'Engineers',

    visibility: { permissions: ['engineers:read'], featureFlags: [CCTV_FEATURE_FLAGS.engineers] },

    items: [

      { id: 'cctv-engineers-list', label: 'Engineers', to: ROUTES.cctv.admin.engineers, icon: 'engineers' },

    ],

  },

  {

    id: 'cctv-billing',

    title: 'Billing',

    visibility: { permissions: ['invoices:read'], featureFlags: [CCTV_FEATURE_FLAGS.invoices] },

    items: [

      { id: 'cctv-invoices', label: 'Invoices', to: ROUTES.cctv.admin.invoices, icon: 'invoices' },

    ],

  },

  {

    id: 'cctv-reporting',

    title: 'Reporting',

    visibility: { permissions: ['reports:read'], featureFlags: [CCTV_FEATURE_FLAGS.reporting] },

    items: [

      { id: 'cctv-reports', label: 'Reports', to: ROUTES.cctv.admin.reports, icon: 'reports' },

    ],

  },

];



export const CCTV_CUSTOMER_NAV: readonly NavigationGroupConfig[] = [

  {

    id: 'cctv-portal',

    title: 'My AMC',

    visibility: { roles: ['Customer'], featureFlags: [CCTV_FEATURE_FLAGS.customerPortal] },

    items: [

      { id: 'cctv-portal-dashboard', label: 'Dashboard', to: ROUTES.cctv.portal.dashboard, icon: 'dashboard' },

      { id: 'cctv-portal-amc', label: 'AMC Details', to: ROUTES.cctv.portal.amc, icon: 'amc' },

      { id: 'cctv-portal-upcoming', label: 'Upcoming Visits', to: ROUTES.cctv.portal.upcomingVisits, icon: 'visits' },

      { id: 'cctv-portal-history', label: 'Service History', to: ROUTES.cctv.portal.serviceHistory, icon: 'schedule' },

      { id: 'cctv-portal-tickets', label: 'Tickets', to: ROUTES.cctv.portal.tickets, icon: 'tickets' },

      { id: 'cctv-portal-invoices', label: 'Invoices', to: ROUTES.cctv.portal.invoices, icon: 'invoices' },

      { id: 'cctv-portal-profile', label: 'Profile', to: ROUTES.cctv.portal.profile, icon: 'profile' },

    ],

  },

];



export const CCTV_ENGINEER_NAV: readonly NavigationGroupConfig[] = [

  {

    id: 'cctv-engineer',

    title: 'Field Work',

    visibility: { roles: ['Engineer'], featureFlags: [CCTV_FEATURE_FLAGS.engineerPortal] },

    items: [

      { id: 'cctv-engineer-dashboard', label: 'My Day', to: ROUTES.cctv.engineer.dashboard, icon: 'dashboard' },

      { id: 'cctv-engineer-visits', label: 'My Visits', to: ROUTES.cctv.engineer.visits, icon: 'visits' },

      { id: 'cctv-engineer-tickets', label: 'My Tickets', to: ROUTES.cctv.engineer.tickets, icon: 'tickets' },

      { id: 'cctv-engineer-profile', label: 'Profile', to: ROUTES.cctv.engineer.profile, icon: 'profile' },

    ],

  },

];


