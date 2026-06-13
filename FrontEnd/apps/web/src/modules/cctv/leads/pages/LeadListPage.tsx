import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { leadsApi } from '../api';
import type { LeadStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;

const STATUS_OPTIONS: readonly LeadStatus[] = [
  'New',
  'Contacted',
  'Qualified',
  'QuotationSent',
  'Negotiation',
  'Won',
  'Lost',
  'Converted',
];

function statusBadgeClass(status: LeadStatus): string {
  const map: Record<LeadStatus, string> = {
    New: 'bg-primary',
    Contacted: 'bg-info text-dark',
    Qualified: 'bg-secondary',
    QuotationSent: 'bg-warning text-dark',
    Negotiation: 'bg-warning text-dark',
    Won: 'bg-success',
    Lost: 'bg-danger',
    Converted: 'bg-dark',
  };
  return map[status];
}

export default function LeadListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<LeadStatus | ''>('');

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
    queryKey: ['cctv', 'leads', filters],
    queryFn: () => leadsApi.list(filters),
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
        title="Lead Pipeline"
        subtitle={`${data?.totalCount ?? 0} lead${(data?.totalCount ?? 0) !== 1 ? 's' : ''} in pipeline`}
      />

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <input
            type="search"
            className="form-control"
            style={{ maxWidth: '280px' }}
            placeholder="Search name, email, number…"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            aria-label="Search leads"
          />
          <select
            className="form-select"
            style={{ maxWidth: '200px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as LeadStatus | '');
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
            <EmptyState
              icon="people"
              title="No leads yet"
              description="Website inquiries and manual leads will appear here."
            />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>Lead #</th>
                  <th>Contact</th>
                  <th>Status</th>
                  <th>Source</th>
                  <th>City</th>
                  <th>Created</th>
                  <th aria-label="Actions" />
                </tr>
              </thead>
              <tbody>
                {items.map((lead) => (
                  <tr key={lead.id}>
                    <td className="font-monospace small">{lead.leadNumber}</td>
                    <td>
                      <div className="fw-semibold">{lead.contactName}</div>
                      <div className="text-muted small">{lead.email}</div>
                    </td>
                    <td>
                      <span className={`badge ${statusBadgeClass(lead.status)}`}>{lead.status}</span>
                    </td>
                    <td className="text-muted small">{lead.source}</td>
                    <td>{lead.city}</td>
                    <td className="text-muted small">
                      {new Date(lead.createdAtUtc).toLocaleDateString()}
                    </td>
                    <td className="text-end">
                      <Link
                        to={ROUTES.cctv.admin.leadDetail.replace(':leadId', lead.id)}
                        className="btn btn-sm btn-outline-secondary"
                      >
                        View
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          {totalPages > 1 && (
            <div className="card-footer d-flex justify-content-between align-items-center">
              <button
                type="button"
                className="btn btn-sm btn-outline-secondary"
                disabled={page <= 1}
                onClick={() => setPage((p) => p - 1)}
              >
                Previous
              </button>
              <span className="text-muted small">
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
          )}
        </div>
      )}
    </div>
  );
}
