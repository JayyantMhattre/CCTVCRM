import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { amcPlansApi } from '../api';
import type { PlanStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;
const STATUS_OPTIONS: readonly PlanStatus[] = ['Active', 'Retired'];

function planDetailPath(planId: string): string {
  return ROUTES.cctv.admin.amcPlanDetail.replace(':planId', planId);
}

export default function AmcPlanListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<PlanStatus | ''>('');

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
    queryKey: ['cctv', 'amc-plans', filters],
    queryFn: () => amcPlansApi.list(filters),
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <div>
      <PageHeader title="AMC Plans" subtitle={`${data?.totalCount ?? 0} plan${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`} />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <input
            type="search"
            className="form-control"
            style={{ maxWidth: '280px' }}
            placeholder="Search code or name…"
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          />
          <select
            className="form-select"
            style={{ maxWidth: '200px' }}
            value={statusFilter}
            onChange={(e) => { setStatusFilter(e.target.value as PlanStatus | ''); setPage(1); }}
          >
            <option value="">All statuses</option>
            {STATUS_OPTIONS.map((s) => <option key={s} value={s}>{s}</option>)}
          </select>
          {isFetching && <span className="text-muted small align-self-center">Updating…</span>}
        </div>
      </div>

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm"><div className="card-body"><EmptyState title="No AMC plans" description="Create a plan to get started." /></div></div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead>
                <tr>
                  <th>Code</th>
                  <th>Name</th>
                  <th>Status</th>
                  <th>Published versions</th>
                  <th>Created</th>
                </tr>
              </thead>
              <tbody>
                {items.map((plan) => (
                  <tr key={plan.id}>
                    <td><Link to={planDetailPath(plan.id)}>{plan.planCode}</Link></td>
                    <td>{plan.name}</td>
                    <td><span className={`badge ${plan.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>{plan.status}</span></td>
                    <td>{plan.publishedVersionCount}</td>
                    <td>{new Date(plan.createdAtUtc).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {totalPages > 1 && (
        <nav className="mt-3 d-flex justify-content-center gap-2">
          <button type="button" className="btn btn-outline-secondary btn-sm" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>Previous</button>
          <span className="align-self-center small text-muted">Page {page} of {totalPages}</span>
          <button type="button" className="btn btn-outline-secondary btn-sm" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>Next</button>
        </nav>
      )}
    </div>
  );
}
