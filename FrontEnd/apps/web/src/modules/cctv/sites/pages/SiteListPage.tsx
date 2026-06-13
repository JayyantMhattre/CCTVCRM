import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { sitesApi } from '../api';
import type { SiteStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;
const STATUS_OPTIONS: readonly SiteStatus[] = ['Active', 'Inactive'];

function siteDetailPath(siteId: string): string {
  return ROUTES.cctv.admin.siteDetail.replace(':siteId', siteId);
}

export default function SiteListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<SiteStatus | ''>('');

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
    queryKey: ['cctv', 'sites', filters],
    queryFn: () => sitesApi.list(filters),
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
        title="Sites"
        subtitle={`${data?.totalCount ?? 0} site${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`}
      />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <input
            type="search"
            className="form-control"
            style={{ maxWidth: '280px' }}
            placeholder="Search name, number, city…"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            aria-label="Search sites"
          />
          <select
            className="form-select"
            style={{ maxWidth: '200px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as SiteStatus | '');
              setPage(1);
            }}
            aria-label="Filter by status"
          >
            <option value="">All statuses</option>
            {STATUS_OPTIONS.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
        </div>
      </div>

      {items.length === 0 ? (
        <EmptyState title="No sites found" description="Sites will appear here once created." />
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Site #</th>
                  <th>Name</th>
                  <th>City</th>
                  <th>Status</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {items.map((site) => (
                  <tr key={site.id}>
                    <td className="font-monospace">{site.siteNumber}</td>
                    <td>{site.name}</td>
                    <td>{site.city}</td>
                    <td>
                      <span
                        className={`badge ${site.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}
                      >
                        {site.status}
                      </span>
                    </td>
                    <td className="text-end">
                      <Link to={siteDetailPath(site.id)} className="btn btn-sm btn-outline-primary">
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
        <nav className="mt-3 d-flex justify-content-between align-items-center" aria-label="Site pagination">
          <button
            type="button"
            className="btn btn-outline-secondary btn-sm"
            disabled={page <= 1 || isFetching}
            onClick={() => setPage((p) => p - 1)}
          >
            Previous
          </button>
          <span className="text-muted small">
            Page {page} of {totalPages}
          </span>
          <button
            type="button"
            className="btn btn-outline-secondary btn-sm"
            disabled={page >= totalPages || isFetching}
            onClick={() => setPage((p) => p + 1)}
          >
            Next
          </button>
        </nav>
      )}
    </div>
  );
}
