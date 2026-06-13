import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { customersApi } from '../api';
import type { CustomerStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

const PAGE_SIZE = 20;

const STATUS_OPTIONS: readonly CustomerStatus[] = ['Active', 'Inactive'];

function statusBadgeClass(status: CustomerStatus): string {
  return status === 'Active' ? 'bg-success' : 'bg-secondary';
}

function customerDetailPath(customerId: string): string {
  return ROUTES.cctv.admin.customerDetail.replace(':customerId', customerId);
}

export default function CustomerListPage() {
  const { extractMessage } = useApiError();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<CustomerStatus | ''>('');

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
    queryKey: ['cctv', 'customers', filters],
    queryFn: () => customersApi.list(filters),
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
        title="Customers"
        subtitle={`${data?.totalCount ?? 0} customer${(data?.totalCount ?? 0) !== 1 ? 's' : ''}`}
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
            aria-label="Search customers"
          />
          <select
            className="form-select"
            style={{ maxWidth: '200px' }}
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value as CustomerStatus | '');
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
              title="No customers yet"
              description="Manual customers and converted leads will appear here."
            />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Number</th>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Phone</th>
                  <th>City</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {items.map((customer) => (
                  <tr key={customer.id}>
                    <td>
                      <Link to={customerDetailPath(customer.id)} className="text-decoration-none fw-medium">
                        {customer.customerNumber}
                      </Link>
                    </td>
                    <td>{customer.name}</td>
                    <td>{customer.email}</td>
                    <td>{customer.phone}</td>
                    <td>{customer.city}</td>
                    <td>
                      <span className={`badge ${statusBadgeClass(customer.status)}`}>{customer.status}</span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {totalPages > 1 && (
        <nav className="mt-3" aria-label="Customer pagination">
          <ul className="pagination mb-0">
            <li className={`page-item ${page <= 1 ? 'disabled' : ''}`}>
              <button type="button" className="page-link" onClick={() => setPage((p) => p - 1)}>
                Previous
              </button>
            </li>
            <li className="page-item disabled">
              <span className="page-link">
                Page {page} of {totalPages}
              </span>
            </li>
            <li className={`page-item ${page >= totalPages ? 'disabled' : ''}`}>
              <button type="button" className="page-link" onClick={() => setPage((p) => p + 1)}>
                Next
              </button>
            </li>
          </ul>
        </nav>
      )}
    </div>
  );
}
