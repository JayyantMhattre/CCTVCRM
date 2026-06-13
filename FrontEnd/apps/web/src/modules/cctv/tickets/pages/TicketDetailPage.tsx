import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { ticketsApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';

export default function TicketDetailPage() {
  const { ticketId = '' } = useParams<{ ticketId: string }>();
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'tickets', ticketId],
    queryFn: () => ticketsApi.get(ticketId),
    enabled: Boolean(ticketId),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  if (!data) return null;

  return (
    <div>
      <PageHeader title={data.ticketNumber} subtitle={data.subject} />

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <h2 className="h6 text-muted">Description</h2>
              <p className="mb-0">{data.description}</p>
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">Comments</h2>
            </div>
            <ul className="list-group list-group-flush">
              {data.comments.length === 0 ? (
                <li className="list-group-item text-muted">No comments yet.</li>
              ) : (
                data.comments.map((comment) => (
                  <li key={comment.id} className="list-group-item">
                    <div className="small text-muted mb-1">
                      {comment.authorRole} · {comment.createdAtUtc}
                    </div>
                    <div>{comment.comment}</div>
                  </li>
                ))
              )}
            </ul>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <dl className="row mb-0 small">
                <dt className="col-5">Status</dt>
                <dd className="col-7">{data.status}</dd>
                <dt className="col-5">Priority</dt>
                <dd className="col-7">{data.priority}</dd>
                <dt className="col-5">Source</dt>
                <dd className="col-7">{data.source}</dd>
                <dt className="col-5">Site</dt>
                <dd className="col-7 text-truncate">{data.siteId}</dd>
                <dt className="col-5">Reopens</dt>
                <dd className="col-7">{data.reopenCount}</dd>
                <dt className="col-5">Engineer</dt>
                <dd className="col-7">{data.assignedEngineerId ?? '—'}</dd>
              </dl>
            </div>
          </div>

          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">Status history</h2>
            </div>
            <ul className="list-group list-group-flush small">
              {data.statusHistory.map((entry) => (
                <li key={entry.id} className="list-group-item">
                  {(entry.fromStatus ?? '—')} → {entry.toStatus}
                  {entry.reason ? ` (${entry.reason})` : ''}
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}
