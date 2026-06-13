import { useQueries } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { portalApi } from '../api';
import { invoicesApi } from '../../invoices/api';
import { ticketsApi } from '../../tickets/api';

export default function PortalDashboardPage() {
  const { extractMessage } = useApiError();

  const [amcQuery, upcomingQuery, ticketsQuery, invoicesQuery] = useQueries({
    queries: [
      { queryKey: ['cctv', 'portal', 'amc'], queryFn: () => portalApi.getAmc() },
      { queryKey: ['cctv', 'portal', 'visits', 'upcoming'], queryFn: () => portalApi.listUpcomingVisits() },
      { queryKey: ['cctv', 'portal', 'tickets', 1], queryFn: () => ticketsApi.listPortal(1, 50) },
      { queryKey: ['cctv', 'portal', 'invoices', 1], queryFn: () => invoicesApi.listPortal(1, 5) },
    ],
  });

  const loading = amcQuery.isLoading || upcomingQuery.isLoading || ticketsQuery.isLoading || invoicesQuery.isLoading;
  const error = amcQuery.error ?? upcomingQuery.error ?? ticketsQuery.error ?? invoicesQuery.error;

  if (loading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;

  const amc = amcQuery.data;
  const upcoming = upcomingQuery.data ?? [];
  const openTickets = (ticketsQuery.data?.items ?? []).filter((t) => t.status !== 'Closed' && t.status !== 'Resolved');
  const recentInvoices = invoicesQuery.data?.items ?? [];

  return (
    <div>
      <PageHeader title="Dashboard" subtitle="Your AMC and service overview" />

      <div className="row g-3 mb-4">
        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <div className="text-muted small">Active AMC</div>
              <div className="fw-semibold">{amc ? amc.planName : 'No active contract'}</div>
              {amc && (
                <div className="small text-muted">
                  {amc.startDate} — {amc.endDate}
                </div>
              )}
              <Link to={ROUTES.cctv.portal.amc} className="btn btn-sm btn-outline-primary mt-2">
                View AMC
              </Link>
            </div>
          </div>
        </div>
        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <div className="text-muted small">Upcoming visits</div>
              <div className="display-6 fw-semibold">{upcoming.length}</div>
              <Link to={ROUTES.cctv.portal.upcomingVisits} className="btn btn-sm btn-outline-primary mt-2">
                View schedule
              </Link>
            </div>
          </div>
        </div>
        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <div className="text-muted small">Open tickets</div>
              <div className="display-6 fw-semibold">{openTickets.length}</div>
              <Link to={ROUTES.cctv.portal.tickets} className="btn btn-sm btn-outline-primary mt-2">
                View tickets
              </Link>
            </div>
          </div>
        </div>
        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <div className="text-muted small">Recent invoices</div>
              <div className="display-6 fw-semibold">{recentInvoices.length}</div>
              <Link to={ROUTES.cctv.portal.invoices} className="btn btn-sm btn-outline-primary mt-2">
                View invoices
              </Link>
            </div>
          </div>
        </div>
      </div>

      <div className="card border-0 shadow-sm mb-4">
        <div className="card-header bg-white">
          <h2 className="h6 mb-0">Quick actions</h2>
        </div>
        <div className="card-body d-flex flex-wrap gap-2">
          <Link to={ROUTES.cctv.portal.ticketCreate} className="btn btn-primary btn-sm">
            Create ticket
          </Link>
          <Link to={ROUTES.cctv.portal.serviceHistory} className="btn btn-outline-secondary btn-sm">
            Service history
          </Link>
          <Link to={ROUTES.cctv.portal.profile} className="btn btn-outline-secondary btn-sm">
            Profile
          </Link>
        </div>
      </div>

      {recentInvoices.length > 0 && (
        <div className="card border-0 shadow-sm">
          <div className="card-header bg-white d-flex justify-content-between align-items-center">
            <h2 className="h6 mb-0">Recent invoices</h2>
            <Link to={ROUTES.cctv.portal.invoices} className="small">
              View all
            </Link>
          </div>
          <ul className="list-group list-group-flush">
            {recentInvoices.map((invoice) => (
              <li key={invoice.id} className="list-group-item d-flex justify-content-between align-items-center">
                <div>
                  <div className="fw-medium">{invoice.invoiceNumber}</div>
                  <div className="small text-muted">{invoice.invoiceDate}</div>
                </div>
                <div className="text-end">
                  <span className="badge bg-info text-dark me-2">{invoice.status}</span>
                  <span>{invoice.totalAmount.toFixed(2)}</span>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
