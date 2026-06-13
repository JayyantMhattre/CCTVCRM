import { Suspense, lazy, type ReactNode } from 'react';

import type { RouteObject } from 'react-router-dom';

import { ROUTES } from '@/core/router/routeMap';

import { RoleGuard } from '@/shared/guards/RoleGuard';

import { Spinner } from '@/shared/components/Spinner';

import { CctvAdminRouteGuard } from './guards/CctvAdminRouteGuard';



const LeadListPage = lazy(() => import('./leads/pages/LeadListPage'));

const LeadDetailPage = lazy(() => import('./leads/pages/LeadDetailPage'));

const CustomerListPage = lazy(() => import('./customers/pages/CustomerListPage'));

const CustomerDetailPage = lazy(() => import('./customers/pages/CustomerDetailPage'));

const SiteListPage = lazy(() => import('./sites/pages/SiteListPage'));

const SiteDetailPage = lazy(() => import('./sites/pages/SiteDetailPage'));

const AmcPlanListPage = lazy(() => import('./amc/plans/pages/AmcPlanListPage'));

const AmcPlanDetailPage = lazy(() => import('./amc/plans/pages/AmcPlanDetailPage'));

const AmcContractListPage = lazy(() => import('./amc/contracts/pages/AmcContractListPage'));

const AmcContractDetailPage = lazy(() => import('./amc/contracts/pages/AmcContractDetailPage'));

const ScheduleListPage = lazy(() => import('./schedules/pages/ScheduleListPage'));

const VisitListPage = lazy(() => import('./visits/pages/VisitListPage'));

const VisitDetailPage = lazy(() => import('./visits/pages/VisitDetailPage'));

const TicketListPage = lazy(() => import('./tickets/pages/TicketListPage'));

const TicketDetailPage = lazy(() => import('./tickets/pages/TicketDetailPage'));

const PortalTicketListPage = lazy(() => import('./tickets/pages/PortalTicketListPage'));

const InvoiceListPage = lazy(() => import('./invoices/pages/InvoiceListPage'));

const InvoiceDetailPage = lazy(() => import('./invoices/pages/InvoiceDetailPage'));

const PortalInvoiceListPage = lazy(() => import('./invoices/pages/PortalInvoiceListPage'));

const EngineerListPage = lazy(() => import('./engineers/pages/EngineerListPage'));

const EngineerCreatePage = lazy(() => import('./engineers/pages/EngineerCreatePage'));

const EngineerDetailPage = lazy(() => import('./engineers/pages/EngineerDetailPage'));

const InvoiceDraftPage = lazy(() => import('./invoices/pages/InvoiceDraftPage'));

const EngineerTicketListPage = lazy(() => import('./tickets/pages/EngineerTicketListPage'));

const EngineerVisitsPage = lazy(() => import('./engineer/pages/EngineerVisitsPage'));

const EngineerDashboardPage = lazy(() => import('./engineer/pages/EngineerDashboardPage'));

const EngineerVisitDetailPage = lazy(() => import('./engineer/pages/EngineerVisitDetailPage'));

const EngineerVisitReportPage = lazy(() => import('./engineer/pages/EngineerVisitReportPage'));

const EngineerTicketDetailPage = lazy(() => import('./tickets/pages/EngineerTicketDetailPage'));

const EngineerProfilePage = lazy(() => import('./engineer/pages/EngineerProfilePage'));

const ReportsHubPage = lazy(() => import('./reports/pages/ReportsHubPage'));

const ReportViewPage = lazy(() => import('./reports/pages/ReportViewPage'));

const PortalDashboardPage = lazy(() => import('./portal/pages/PortalDashboardPage'));

const PortalAmcPage = lazy(() => import('./portal/pages/PortalAmcPage'));

const PortalServiceHistoryPage = lazy(() => import('./portal/pages/PortalServiceHistoryPage'));

const PortalUpcomingVisitsPage = lazy(() => import('./portal/pages/PortalUpcomingVisitsPage'));

const PortalTicketCreatePage = lazy(() => import('./portal/pages/PortalTicketCreatePage'));

const PortalTicketDetailPage = lazy(() => import('./portal/pages/PortalTicketDetailPage'));

const PortalInvoiceDetailPage = lazy(() => import('./portal/pages/PortalInvoiceDetailPage'));

const PortalProfilePage = lazy(() => import('./portal/pages/PortalProfilePage'));



const leadPage = (element: ReactNode) => (

  <Suspense fallback={<Spinner fullPage />}>{element}</Suspense>

);



/** CCTV admin routes — Lead pipeline (B1) + placeholders for B2–B7. */

export const cctvAdminRoutes: RouteObject[] = [

  {

    element: <CctvAdminRouteGuard />,

    children: [

      { path: ROUTES.cctv.admin.leads, element: leadPage(<LeadListPage />) },

      { path: ROUTES.cctv.admin.leadDetail, element: leadPage(<LeadDetailPage />) },

      { path: ROUTES.cctv.admin.customers, element: leadPage(<CustomerListPage />) },

      { path: ROUTES.cctv.admin.customerDetail, element: leadPage(<CustomerDetailPage />) },

      { path: ROUTES.cctv.admin.sites, element: leadPage(<SiteListPage />) },

      { path: ROUTES.cctv.admin.siteDetail, element: leadPage(<SiteDetailPage />) },

      { path: ROUTES.cctv.admin.amcPlans, element: leadPage(<AmcPlanListPage />) },

      { path: ROUTES.cctv.admin.amcPlanDetail, element: leadPage(<AmcPlanDetailPage />) },

      { path: ROUTES.cctv.admin.amcContracts, element: leadPage(<AmcContractListPage />) },

      { path: ROUTES.cctv.admin.amcContractDetail, element: leadPage(<AmcContractDetailPage />) },

      { path: ROUTES.cctv.admin.schedules, element: leadPage(<ScheduleListPage />) },

      { path: ROUTES.cctv.admin.visits, element: leadPage(<VisitListPage />) },

      { path: ROUTES.cctv.admin.visitDetail, element: leadPage(<VisitDetailPage />) },

      { path: ROUTES.cctv.admin.tickets, element: leadPage(<TicketListPage />) },

      { path: ROUTES.cctv.admin.ticketDetail, element: leadPage(<TicketDetailPage />) },

      { path: ROUTES.cctv.admin.invoices, element: leadPage(<InvoiceListPage />) },

      { path: ROUTES.cctv.admin.invoiceCreate, element: leadPage(<InvoiceDraftPage />) },

      { path: ROUTES.cctv.admin.invoiceEdit, element: leadPage(<InvoiceDraftPage />) },

      { path: ROUTES.cctv.admin.invoiceDetail, element: leadPage(<InvoiceDetailPage />) },

      { path: ROUTES.cctv.admin.engineers, element: leadPage(<EngineerListPage />) },

      { path: ROUTES.cctv.admin.engineerCreate, element: leadPage(<EngineerCreatePage />) },

      { path: ROUTES.cctv.admin.engineerDetail, element: leadPage(<EngineerDetailPage />) },

      { path: ROUTES.cctv.admin.reports, element: leadPage(<ReportsHubPage />) },

      { path: ROUTES.cctv.admin.reportView, element: leadPage(<ReportViewPage />) },

    ],

  },

];



export const cctvPortalRoutes: RouteObject[] = [

  {

    element: <RoleGuard roles={['Customer']} />,

    children: [

      { path: ROUTES.cctv.portal.dashboard, element: leadPage(<PortalDashboardPage />) },

      { path: ROUTES.cctv.portal.amc, element: leadPage(<PortalAmcPage />) },

      { path: ROUTES.cctv.portal.serviceHistory, element: leadPage(<PortalServiceHistoryPage />) },

      { path: ROUTES.cctv.portal.upcomingVisits, element: leadPage(<PortalUpcomingVisitsPage />) },

      { path: ROUTES.cctv.portal.tickets, element: leadPage(<PortalTicketListPage />) },

      { path: ROUTES.cctv.portal.ticketCreate, element: leadPage(<PortalTicketCreatePage />) },

      { path: ROUTES.cctv.portal.ticketDetail, element: leadPage(<PortalTicketDetailPage />) },

      { path: ROUTES.cctv.portal.invoices, element: leadPage(<PortalInvoiceListPage />) },

      { path: ROUTES.cctv.portal.invoiceDetail, element: leadPage(<PortalInvoiceDetailPage />) },

      { path: ROUTES.cctv.portal.profile, element: leadPage(<PortalProfilePage />) },

    ],

  },

];



export const cctvEngineerRoutes: RouteObject[] = [

  {

    element: <RoleGuard roles={['Engineer']} />,

    children: [

      { path: ROUTES.cctv.engineer.dashboard, element: leadPage(<EngineerDashboardPage />) },

      { path: ROUTES.cctv.engineer.visits, element: leadPage(<EngineerVisitsPage />) },

      { path: ROUTES.cctv.engineer.visitDetail, element: leadPage(<EngineerVisitDetailPage />) },

      { path: ROUTES.cctv.engineer.visitReport, element: leadPage(<EngineerVisitReportPage />) },

      { path: ROUTES.cctv.engineer.tickets, element: leadPage(<EngineerTicketListPage />) },

      { path: ROUTES.cctv.engineer.ticketDetail, element: leadPage(<EngineerTicketDetailPage />) },

      { path: ROUTES.cctv.engineer.profile, element: leadPage(<EngineerProfilePage />) },

    ],

  },

];


