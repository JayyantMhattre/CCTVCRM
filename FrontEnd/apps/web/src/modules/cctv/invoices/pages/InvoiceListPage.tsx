import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { invoicesApi } from '../api';
import type { InvoiceStatus, InvoiceType } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;
const STATUS_OPTIONS: readonly InvoiceStatus[] = ['Draft', 'Generated', 'Sent', 'Paid', 'Cancelled'];
const TYPE_OPTIONS: readonly InvoiceType[] = [
  'AmcRenewal',
  'NewAmc',
  'EmergencyService',
  'SpareReplacement',
  'AdditionalCharges',
  'Other',
];

function invoiceDetailPath(invoiceId: string): string {
  return ROUTES.cctv.admin.invoiceDetail.replace(':invoiceId', invoiceId);
}

export default function InvoiceListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<InvoiceStatus | ''>('');
  const [typeFilter, setTypeFilter] = useState<InvoiceType | ''>('');

  const filters = useMemo(
    () => ({
      page,
      pageSize: PAGE_SIZE,
      status: statusFilter || undefined,
      invoiceType: typeFilter || undefined,
    }),
    [page, statusFilter, typeFilter],
  );

  const { data, isLoading, error, isFetching } = useQuery({
    queryKey: ['cctv', 'invoices', filters],
    queryFn: () => invoicesApi.list(filters),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <div>
      <PageHeader title="Invoices" subtitle={`${data?.totalCount ?? 0} invoice${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`}>
        <Link to={ROUTES.cctv.admin.invoiceCreate} className="btn btn-primary btn-sm">
          New draft
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body d-flex flex-wrap gap-2">
          <select
            className="form-select"
            style={{ maxWidth: '220px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as InvoiceStatus | '');
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
          <select
            className="form-select"
            style={{ maxWidth: '240px' }}
            value={typeFilter}
            onChange={(e) => {
              setTypeFilter(e.target.value as InvoiceType | '');
              setPage(1);
            }}
            aria-label="Filter by type"
          >
            <option value="">All types</option>
            {TYPE_OPTIONS.map((type) => (
              <option key={type} value={type}>
                {type}
              </option>
            ))}
          </select>
          {isFetching && <span className="text-muted small align-self-center">Updating…</span>}
        </div>
      </div>

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No invoices" description="Draft invoices appear here once created by an admin." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Number</th>
                  <th>Type</th>
                  <th>Status</th>
                  <th>Date</th>
                  <th className="text-end">Total</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {items.map((invoice) => (
                  <tr key={invoice.id}>
                    <td className="fw-medium">{invoice.invoiceNumber}</td>
                    <td>{invoice.invoiceType}</td>
                    <td>
                      <span className="badge bg-info text-dark">{invoice.status}</span>
                    </td>
                    <td className="text-muted small">{invoice.invoiceDate}</td>
                    <td className="text-end">{invoice.totalAmount.toFixed(2)}</td>
                    <td>
                      <Link to={invoiceDetailPath(invoice.id)} className="btn btn-sm btn-outline-primary">
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
