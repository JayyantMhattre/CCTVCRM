import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { amcContractsApi } from '../api';
import type { ContractStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;
const STATUS_OPTIONS: readonly ContractStatus[] = ['Active', 'Expired', 'Cancelled'];

function contractDetailPath(contractId: string): string {
  return ROUTES.cctv.admin.amcContractDetail.replace(':contractId', contractId);
}

export default function AmcContractListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<ContractStatus | ''>('');

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
    queryKey: ['cctv', 'amc-contracts', filters],
    queryFn: () => amcContractsApi.list(filters),
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;

  const items = data?.items ?? [];

  return (
    <div>
      <PageHeader title="AMC Contracts" subtitle={`${data?.totalCount ?? 0} contract${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`} />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <input type="search" className="form-control" style={{ maxWidth: '280px' }} placeholder="Search contract number…" value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} />
          <select className="form-select" style={{ maxWidth: '200px' }} value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value as ContractStatus | ''); setPage(1); }}>
            <option value="">All statuses</option>
            {STATUS_OPTIONS.map((s) => <option key={s} value={s}>{s}</option>)}
          </select>
          {isFetching && <span className="text-muted small align-self-center">Updating…</span>}
        </div>
      </div>

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm"><div className="card-body"><EmptyState title="No AMC contracts" description="Contracts appear after lead conversion or manual creation." /></div></div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead>
                <tr>
                  <th>Number</th>
                  <th>Plan</th>
                  <th>Status</th>
                  <th>Term ends</th>
                  <th>Created</th>
                </tr>
              </thead>
              <tbody>
                {items.map((c) => (
                  <tr key={c.id}>
                    <td><Link to={contractDetailPath(c.id)}>{c.contractNumber}</Link></td>
                    <td>{c.planCode ?? '—'}</td>
                    <td><span className={`badge ${c.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>{c.status}</span></td>
                    <td>{c.activeTermEndDate ?? '—'}</td>
                    <td>{new Date(c.createdAtUtc).toLocaleDateString()}</td>
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
