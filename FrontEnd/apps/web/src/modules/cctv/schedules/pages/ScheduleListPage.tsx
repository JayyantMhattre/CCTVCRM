import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { schedulesApi } from '../api';
import type { ScheduleStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';

const PAGE_SIZE = 20;
const STATUS_OPTIONS: readonly ScheduleStatus[] = [
  'Planned',
  'Assigned',
  'InProgress',
  'Completed',
  'Missed',
  'Cancelled',
];

function statusBadgeClass(status: ScheduleStatus): string {
  switch (status) {
    case 'Completed':
      return 'bg-success';
    case 'Cancelled':
    case 'Missed':
      return 'bg-danger';
    case 'InProgress':
      return 'bg-primary';
    case 'Assigned':
      return 'bg-info';
    default:
      return 'bg-secondary';
  }
}

export default function ScheduleListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<ScheduleStatus | ''>('');

  const filters = useMemo(
    () => ({
      page,
      pageSize: PAGE_SIZE,
      status: statusFilter || undefined,
    }),
    [page, statusFilter],
  );

  const { data, isLoading, error, isFetching } = useQuery({
    queryKey: ['cctv', 'schedules', filters],
    queryFn: () => schedulesApi.list(filters),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <div>
      <PageHeader
        title="Schedules"
        subtitle={`${data?.totalCount ?? 0} schedule${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`}
      />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <select
            className="form-select"
            style={{ maxWidth: '220px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as ScheduleStatus | '');
              setPage(1);
            }}
            aria-label="Filter by status"
          >
            <option value="">All statuses</option>
            {STATUS_OPTIONS.map((status) => (
              <option key={status} value={status}>
                {status}
              </option>
            ))}
          </select>
          {isFetching && <span className="text-muted small align-self-center">Updating…</span>}
        </div>
      </div>

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No schedules" description="Service schedules will appear here once AMC terms are activated." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Number</th>
                  <th>Date</th>
                  <th>Status</th>
                  <th>Site</th>
                  <th>Seq</th>
                </tr>
              </thead>
              <tbody>
                {items.map((schedule) => (
                  <tr key={schedule.id}>
                    <td className="fw-medium">{schedule.scheduleNumber}</td>
                    <td>{schedule.scheduledDate}</td>
                    <td>
                      <span className={`badge ${statusBadgeClass(schedule.status)}`}>{schedule.status}</span>
                    </td>
                    <td className="text-muted small">{schedule.siteId.slice(0, 8)}…</td>
                    <td>{schedule.sequenceInTerm || '—'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {totalPages > 1 && (
        <div className="d-flex justify-content-between align-items-center mt-3">
          <button type="button" className="btn btn-outline-secondary btn-sm" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
            Previous
          </button>
          <span className="text-muted small">
            Page {page} of {totalPages}
          </span>
          <button
            type="button"
            className="btn btn-outline-secondary btn-sm"
            disabled={page >= totalPages}
            onClick={() => setPage((p) => p + 1)}
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
