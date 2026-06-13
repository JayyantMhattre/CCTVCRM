import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { ticketsApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;

function ticketDetailPath(ticketId: string): string {
  return ROUTES.cctv.engineer.ticketDetail.replace(':ticketId', ticketId);
}

export default function EngineerTicketListPage() {
  const { extractMessage } = useApiError();
  const page = 1;

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'engineer', 'tickets', page],
    queryFn: () => ticketsApi.listEngineer(page, PAGE_SIZE),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  const items = data?.items ?? [];

  return (
    <div>
      <PageHeader title="Assigned Tickets" subtitle="Tickets assigned to you" />

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No assigned tickets" description="Tickets appear here when an admin assigns you." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Number</th>
                  <th>Subject</th>
                  <th>Priority</th>
                  <th>Status</th>
                  <th className="text-end">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((ticket) => (
                  <tr key={ticket.id}>
                    <td className="fw-medium">{ticket.ticketNumber}</td>
                    <td>{ticket.subject}</td>
                    <td>{ticket.priority}</td>
                    <td>{ticket.status}</td>
                    <td className="text-end">
                      <Link to={ticketDetailPath(ticket.id)} className="btn btn-sm btn-outline-primary">
                        Open
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
