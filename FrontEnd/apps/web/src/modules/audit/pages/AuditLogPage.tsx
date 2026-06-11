/**
 * AuditLogPage — admin audit viewer with filters, pagination, and loading/empty states.
 */

import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { auditApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import type { AuditEventType, AuditLogFilters } from '../types';

const EVENT_BADGE: Record<AuditEventType, string> = {
  ApiCall:      'bg-primary',
  EntityChange: 'bg-info text-dark',
  UserAction:   'bg-success',
  DomainEvent:  'bg-secondary',
};

const EVENT_TYPES: AuditEventType[] = [
  'ApiCall',
  'EntityChange',
  'UserAction',
  'DomainEvent',
];

const PAGE_SIZE = 25;

export default function AuditLogPage() {
  const { extractMessage, extractCorrelationId } = useApiError();

  const [moduleFilter, setModuleFilter] = useState('');
  const [eventType, setEventType] = useState<AuditEventType | ''>('');
  const [search, setSearch] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [page, setPage] = useState(1);

  const filters: AuditLogFilters = useMemo(
    () => ({
      module: moduleFilter || undefined,
      eventType: eventType || undefined,
      search: search || undefined,
      from: fromDate ? new Date(fromDate).toISOString() : undefined,
      to: toDate ? new Date(`${toDate}T23:59:59`).toISOString() : undefined,
      page,
      pageSize: PAGE_SIZE,
    }),
    [moduleFilter, eventType, search, fromDate, toDate, page],
  );

  const { data, isLoading, error, isFetching } = useQuery({
    queryKey: ['audit', 'logs', filters],
    queryFn: () => auditApi.getLogs({ ...filters, page, pageSize: PAGE_SIZE }),
    staleTime: 0,
    gcTime: 30_000,
  });

  const entries = data?.items ?? [];
  const totalCount = data?.totalCount ?? entries.length;
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE) || 1);
  const pageItems = entries;

  function resetFilters() {
    setModuleFilter('');
    setEventType('');
    setSearch('');
    setFromDate('');
    setToDate('');
    setPage(1);
  }

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
      <PageHeader
        title="Audit Logs"
        subtitle="Tamper-evident record of platform activity."
      />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body py-3">
          <div className="row g-2">
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">Module</label>
              <input
                type="text"
                className="form-control form-control-sm"
                placeholder="e.g. Auth"
                value={moduleFilter}
                onChange={(e) => { setModuleFilter(e.target.value); setPage(1); }}
              />
            </div>
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">Event type</label>
              <select
                className="form-select form-select-sm"
                value={eventType}
                onChange={(e) => { setEventType(e.target.value as AuditEventType | ''); setPage(1); }}
              >
                <option value="">All types</option>
                {EVENT_TYPES.map((t) => (
                  <option key={t} value={t}>{t}</option>
                ))}
              </select>
            </div>
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">Search</label>
              <input
                type="search"
                className="form-control form-control-sm"
                placeholder="Action, module, user id…"
                value={search}
                onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              />
            </div>
            <div className="col-md-3 d-flex align-items-end">
              <button
                type="button"
                className="btn btn-outline-secondary btn-sm"
                onClick={resetFilters}
              >
                Clear filters
              </button>
            </div>
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">From</label>
              <input
                type="date"
                className="form-control form-control-sm"
                value={fromDate}
                onChange={(e) => { setFromDate(e.target.value); setPage(1); }}
              />
            </div>
            <div className="col-md-3">
              <label className="form-label small text-muted mb-0">To</label>
              <input
                type="date"
                className="form-control form-control-sm"
                value={toDate}
                onChange={(e) => { setToDate(e.target.value); setPage(1); }}
              />
            </div>
            <div className="col-md-6 d-flex align-items-end justify-content-end text-muted small">
              {isFetching ? 'Refreshing…' : `Showing ${pageItems.length} of ${totalCount} entries`}
            </div>
          </div>
        </div>
      </div>

      {entries.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState
              icon="journal-check"
              title="No audit entries found"
              description="Adjust filters or check back after platform activity is recorded."
            />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0 small">
              <thead className="table-light">
                <tr>
                  <th>Time</th>
                  <th>Type</th>
                  <th>Module</th>
                  <th>Action</th>
                  <th>User</th>
                  <th>IP</th>
                </tr>
              </thead>
              <tbody>
                {pageItems.map((entry) => (
                  <tr key={entry.id}>
                    <td className="text-muted font-monospace" style={{ whiteSpace: 'nowrap' }}>
                      {new Date(entry.occurredOnUtc).toLocaleString()}
                    </td>
                    <td>
                      <span className={`badge ${EVENT_BADGE[entry.eventType]}`}>
                        {entry.eventType}
                      </span>
                    </td>
                    <td className="fw-semibold">{entry.module}</td>
                    <td>{entry.action}</td>
                    <td className="font-monospace text-muted" style={{ fontSize: '0.7rem' }}>
                      {entry.userId ?? '—'}
                    </td>
                    <td className="text-muted">{entry.ipAddress ?? '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="card-footer d-flex justify-content-between align-items-center">
            <button
              type="button"
              className="btn btn-sm btn-outline-secondary"
              disabled={page <= 1}
              onClick={() => setPage((p) => p - 1)}
            >
              Previous
            </button>
            <span className="small text-muted">
              Page {page} of {totalPages}
            </span>
            <button
              type="button"
              className="btn btn-sm btn-outline-secondary"
              disabled={page >= totalPages}
              onClick={() => setPage((p) => p + 1)}
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
