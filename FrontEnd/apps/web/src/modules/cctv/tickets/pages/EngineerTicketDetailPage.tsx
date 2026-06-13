import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { ticketsApi } from '../api';
import type { TicketStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

const PROGRESS_STATUSES: readonly TicketStatus[] = ['InProgress', 'Resolved'];

export default function EngineerTicketDetailPage() {
  const { ticketId = '' } = useParams<{ ticketId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();

  const ticketQuery = useQuery({
    queryKey: ['cctv', 'tickets', ticketId],
    queryFn: () => ticketsApi.get(ticketId),
    enabled: Boolean(ticketId),
  });

  const statusMutation = useMutation({
    mutationFn: (status: TicketStatus) =>
      ticketsApi.updateStatus(ticketId, { status, rowVersion: ticketQuery.data!.rowVersion }),
    onSuccess: () => {
      toast.success('Ticket status updated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'tickets'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (ticketQuery.isLoading) return <Spinner fullPage />;
  if (ticketQuery.error) return <AlertMessage message={extractMessage(ticketQuery.error)} variant="danger" />;

  const ticket = ticketQuery.data;
  if (!ticket) return null;

  return (
    <div>
      <PageHeader title={ticket.subject} subtitle={ticket.ticketNumber}>
        <Link to={ROUTES.cctv.engineer.tickets} className="btn btn-outline-secondary btn-sm">
          Back to tickets
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          <span className={`badge bg-secondary me-2`}>{ticket.status}</span>
          <span className="badge bg-warning text-dark">{ticket.priority}</span>
          <p className="mt-3 mb-0">{ticket.description}</p>
        </div>
      </div>

      <div className="d-flex flex-wrap gap-2">
        {PROGRESS_STATUSES.map((status) => (
          <button
            key={status}
            type="button"
            className="btn btn-sm btn-outline-primary"
            disabled={statusMutation.isPending || ticket.status === status}
            onClick={() => statusMutation.mutate(status)}
          >
            Mark {status}
          </button>
        ))}
      </div>
    </div>
  );
}
