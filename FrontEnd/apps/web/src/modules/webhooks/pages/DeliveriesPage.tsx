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
import { webhookDeliveryRoute } from '@/core/router/routeMap';
import { DeliveryStatusBadge } from '../components/StatusBadge';
import { deliveryDurationMs, formatDuration, formatUtc } from '../utils/format';
import type { WebhookDeliveryStatus } from '../types';

const STATUSES: WebhookDeliveryStatus[] = ['Pending', 'Succeeded', 'Failed', 'Retrying', 'DeadLettered'];

export default function DeliveriesPage() {
  const { extractMessage, extractCorrelationId } = useApiError();
  const [eventName, setEventName] = useState('');
  const [status, setStatus] = useState<WebhookDeliveryStatus | ''>('');
  const [subscriptionId, setSubscriptionId] = useState('');
  const [search, setSearch] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');

  const filters = useMemo(
    () => ({
      eventName: eventName || undefined,
      status: status || undefined,
      subscriptionId: subscriptionId || undefined,
      fromUtc: fromDate ? new Date(fromDate).toISOString() : undefined,
      toUtc: toDate ? new Date(`${toDate}T23:59:59`).toISOString() : undefined,
      limit: 100,
    }),
    [eventName, status, subscriptionId, fromDate, toDate],
  );

  const { data = [], isLoading, error, isFetching } = useQuery({
    queryKey: ['webhooks', 'deliveries', filters],
    queryFn: () => webhooksApi.listDeliveries(filters),
  });

  const filtered = useMemo(() => {
    if (!search.trim()) return data;
    const q = search.toLowerCase();
    return data.filter(
      (d) =>
        d.eventName.toLowerCase().includes(q) ||
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
      <PageHeader title="Delivery history" subtitle="Investigate webhook delivery outcomes." />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body py-3">
          <div className="row g-2">
            <div className="col-md-2">
              <label className="form-label small text-muted mb-0">Event</label>
              <input className="form-control form-control-sm" value={eventName} onChange={(e) => setEventName(e.target.value)} />
            </div>
            <div className="col-md-2">
              <label className="form-label small text-muted mb-0">Status</label>
              <select className="form-select form-select-sm" value={status} onChange={(e) => setStatus(e.target.value as WebhookDeliveryStatus | '')}>
                <option value="">All</option>
                {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
              </select>
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
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">Search</label>
              <input className="form-control form-control-sm" placeholder="Event or correlation" value={search} onChange={(e) => setSearch(e.target.value)} />
            </div>
          </div>
          {isFetching && <div className="small text-muted mt-2">Refreshing…</div>}
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState icon="inbox" title="No deliveries" description="Adjust filters or wait for webhook events." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0 small">
              <thead className="table-light">
                <tr>
                  <th>Event</th>
                  <th>Status</th>
                  <th>Code</th>
                  <th>Duration</th>
                  <th>Started</th>
                  <th>Correlation</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {filtered.map((d) => (
                  <tr key={d.id}>
                    <td>{d.eventName}</td>
                    <td><DeliveryStatusBadge status={d.status} /></td>
                    <td>{d.responseCode ?? '—'}</td>
                    <td>{formatDuration(deliveryDurationMs(d.startedOnUtc, d.completedOnUtc))}</td>
                    <td>{formatUtc(d.startedOnUtc)}</td>
                    <td className="font-monospace">{d.correlationId ?? '—'}</td>
                    <td className="text-end">
                      <Link to={webhookDeliveryRoute(d.id)} className="btn btn-sm btn-outline-secondary">Details</Link>
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
