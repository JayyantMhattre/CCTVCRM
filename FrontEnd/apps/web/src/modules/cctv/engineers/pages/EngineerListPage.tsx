import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { engineersApi } from '../api';
import type { EngineerStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;

const STATUS_OPTIONS: readonly EngineerStatus[] = ['Active', 'Inactive'];

function statusBadgeClass(status: EngineerStatus): string {
  return status === 'Active' ? 'bg-success' : 'bg-secondary';
}

function engineerDetailPath(engineerId: string): string {
  return ROUTES.cctv.admin.engineerDetail.replace(':engineerId', engineerId);
}

export default function EngineerListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<EngineerStatus | ''>('');

  const filters = useMemo(
    () => ({
      page,
      pageSize: PAGE_SIZE,
      status: statusFilter || undefined,
      search: search.trim() || undefined,
    }),
    [page, search, statusFilter],
  );

  const { data, isLoading, error, isFetching } = useQuery({
    queryKey: ['cctv', 'engineers', filters],
    queryFn: () => engineersApi.list(filters),
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
        title="Engineers"
        subtitle={`${data?.totalCount ?? 0} engineer${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`}
      >
        <Link to={ROUTES.cctv.admin.engineerCreate} className="btn btn-primary btn-sm">
          New engineer
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <input
            type="search"
            className="form-control"
            style={{ maxWidth: '280px' }}
            placeholder="Search name, phone, number…"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            aria-label="Search engineers"
          />
          <select
            className="form-select"
            style={{ maxWidth: '200px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as EngineerStatus | '');
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
        </div>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="card-body p-0">
          {items.length === 0 ? (
            <EmptyState title="No engineers" description="Create engineer records to assign visits and tickets." />
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0">
                <thead className="table-light">
                  <tr>
                    <th>Number</th>
                    <th>Name</th>
                    <th>Phone</th>
                    <th>Status</th>
                    <th className="text-end">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((engineer) => (
                    <tr key={engineer.id}>
                      <td className="font-monospace">{engineer.engineerNumber}</td>
                      <td>{engineer.name}</td>
                      <td>{engineer.phone}</td>
                      <td>
                        <span className={`badge ${statusBadgeClass(engineer.status)}`}>{engineer.status}</span>
                      </td>
                      <td className="text-end">
                        <Link to={engineerDetailPath(engineer.id)} className="btn btn-sm btn-outline-primary">
                          Open
                        </Link>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
        {totalPages > 1 && (
          <div className="card-footer bg-white d-flex justify-content-between align-items-center">
            <span className="text-muted small">
              Page {page} of {totalPages}
              {isFetching ? ' · Loading…' : ''}
            </span>
            <div className="btn-group btn-group-sm">
              <button type="button" className="btn btn-outline-secondary" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
                Previous
              </button>
              <button
                type="button"
                className="btn btn-outline-secondary"
                disabled={page >= totalPages}
                onClick={() => setPage((p) => p + 1)}
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
