import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { webhooksApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { webhookDeadLetterRoute } from '@/core/router/routeMap';
import { formatUtc } from '../utils/format';

export default function DeadLettersPage() {
  const { extractMessage, extractCorrelationId } = useApiError();
  const [eventName, setEventName] = useState('');
  const [subscriptionId, setSubscriptionId] = useState('');
  const [search, setSearch] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');

  const filters = useMemo(
    () => ({
      eventName: eventName || undefined,
      subscriptionId: subscriptionId || undefined,
      fromUtc: fromDate ? new Date(fromDate).toISOString() : undefined,
      toUtc: toDate ? new Date(`${toDate}T23:59:59`).toISOString() : undefined,
      limit: 100,
    }),
    [eventName, subscriptionId, fromDate, toDate],
  );

  const { data = [], isLoading, error } = useQuery({
    queryKey: ['webhooks', 'deadletters', filters],
    queryFn: () => webhooksApi.listDeadLetters(filters),
  });

  const filtered = useMemo(() => {
    if (!search.trim()) return data;
    const q = search.toLowerCase();
    return data.filter(
      (d) =>
        d.eventName.toLowerCase().includes(q) ||
        (d.failureReason?.toLowerCase().includes(q) ?? false) ||
        (d.correlationId?.toLowerCase().includes(q) ?? false),
    );
  }, [data, search]);

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    const correlationId = extractCorrelationId(error);
    return (
      <div>
        <AlertMessage message={extractMessage(error)} variant="danger" />
        {correlationId && <CorrelationIdCopy correlationId={correlationId} className="mt-2" />}
      </div>
    );
  }

  return (
    <div>
      <PageHeader title="Dead letter queue" subtitle="Exhausted deliveries awaiting recovery." />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body py-3">
          <div className="row g-2">
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">Event</label>
              <input className="form-control form-control-sm" value={eventName} onChange={(e) => setEventName(e.target.value)} />
            </div>
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">Subscription ID</label>
              <input className="form-control form-control-sm" value={subscriptionId} onChange={(e) => setSubscriptionId(e.target.value)} />
            </div>
            <div className="col-md-2">
              <label className="form-label small text-muted mb-0">From</label>
              <input type="date" className="form-control form-control-sm" value={fromDate} onChange={(e) => setFromDate(e.target.value)} />
            </div>
            <div className="col-md-2">
              <label className="form-label small text-muted mb-0">To</label>
              <input type="date" className="form-control form-control-sm" value={toDate} onChange={(e) => setToDate(e.target.value)} />
            </div>
            <div className="col-md-4">
              <label className="form-label small text-muted mb-0">Search</label>
              <input className="form-control form-control-sm" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Event, reason, correlation" />
            </div>
          </div>
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState icon="exclamation-triangle" title="No dead letters" description="Great — no exhausted deliveries in the current window." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0 small">
              <thead className="table-light">
                <tr>
                  <th>Event</th>
                  <th>Failure</th>
                  <th>Code</th>
                  <th>Retries</th>
                  <th>Created</th>
                  <th>Correlation</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {filtered.map((d) => (
                  <tr key={d.id}>
                    <td>{d.eventName}</td>
                    <td className="text-truncate" style={{ maxWidth: 200 }}>{d.failureReason ?? '—'}</td>
                    <td>{d.failureCode ?? '—'}</td>
                    <td>{d.retryCount}</td>
                    <td>{formatUtc(d.createdOnUtc)}</td>
                    <td className="font-monospace">{d.correlationId ?? '—'}</td>
                    <td className="text-end">
                      <Link to={webhookDeadLetterRoute(d.id)} className="btn btn-sm btn-outline-secondary">Details</Link>
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
