import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { visitsApi } from '../api';
import type { VisitReportStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;
const STATUS_OPTIONS: readonly VisitReportStatus[] = ['Draft', 'Submitted', 'Returned', 'Approved'];

function visitDetailPath(visitId: string): string {
  return ROUTES.cctv.admin.visitDetail.replace(':visitId', visitId);
}

export default function VisitListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<VisitReportStatus | ''>('');

  const filters = useMemo(
    () => ({
      page,
      pageSize: PAGE_SIZE,
      reportStatus: statusFilter || undefined,
    }),
    [page, statusFilter],
  );

  const { data, isLoading, error, isFetching } = useQuery({
    queryKey: ['cctv', 'visits', filters],
    queryFn: () => visitsApi.list(filters),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <div>
      <PageHeader title="Visits" subtitle={`${data?.totalCount ?? 0} visit${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`} />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <select
            className="form-select"
            style={{ maxWidth: '220px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as VisitReportStatus | '');
              setPage(1);
            }}
            aria-label="Filter by report status"
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
            <EmptyState title="No visits" description="Service visits appear when engineers execute assigned schedules." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Schedule</th>
                  <th>Status</th>
                  <th>Started</th>
                  <th>Site</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {items.map((visit) => (
                  <tr key={visit.id}>
                    <td className="fw-medium">{visit.scheduleNumber}</td>
                    <td>
                      <span className="badge bg-secondary">{visit.reportStatus}</span>
                    </td>
                    <td className="text-muted small">{visit.startedAtUtc ?? '—'}</td>
                    <td className="text-muted small">{visit.siteId.slice(0, 8)}…</td>
                    <td>
                      <Link to={visitDetailPath(visit.id)} className="btn btn-sm btn-outline-primary">
                        View
                      </Link>
                    </td>
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
          <button type="button" className="btn btn-outline-secondary btn-sm" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>
            Next
          </button>
        </div>
      )}
    </div>
  );
}
