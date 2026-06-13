import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { invoicesApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;

export default function PortalInvoiceListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'portal', 'invoices', page],
    queryFn: () => invoicesApi.listPortal(page, PAGE_SIZE),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  const items = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <div>
      <PageHeader title="My Invoices" subtitle={`${data?.totalCount ?? 0} invoice${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`} />

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No invoices" description="Your generated invoices will appear here." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Number</th>
                  <th>Status</th>
                  <th>Date</th>
                  <th className="text-end">Amount</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {items.map((invoice) => (
                  <tr key={invoice.id}>
                    <td className="fw-medium">{invoice.invoiceNumber}</td>
                    <td>
                      <span className="badge bg-info text-dark">{invoice.status}</span>
                    </td>
                    <td className="text-muted small">{invoice.invoiceDate}</td>
                    <td className="text-end">{invoice.totalAmount.toFixed(2)}</td>
                    <td>
                      <Link
                        to={ROUTES.cctv.portal.invoiceDetail.replace(':invoiceId', invoice.id)}
                        className="btn btn-sm btn-outline-primary"
                      >
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
