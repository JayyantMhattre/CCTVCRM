import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { customersApi } from '../api';
import type { CustomerStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

export default function CustomerDetailPage() {
  const { customerId = '' } = useParams<{ customerId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();

  const customerQuery = useQuery({
    queryKey: ['cctv', 'customers', customerId],
    queryFn: () => customersApi.get(customerId),
    enabled: Boolean(customerId),
  });

  const statusMutation = useMutation({
    mutationFn: (status: CustomerStatus) =>
      customersApi.changeStatus(customerId, {
        status,
        rowVersion: customerQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Customer status updated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'customers'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (customerQuery.isLoading) return <Spinner fullPage />;

  if (customerQuery.error) {
    return <AlertMessage message={extractMessage(customerQuery.error)} variant="danger" />;
  }

  const customer = customerQuery.data;
  if (!customer) return null;

  const nextStatus: CustomerStatus | null =
    customer.status === 'Active' ? 'Inactive' : customer.status === 'Inactive' ? 'Active' : null;

  return (
    <div>
      <PageHeader title={customer.name} subtitle={customer.customerNumber}>
        <Link to={ROUTES.cctv.admin.customers} className="btn btn-outline-secondary btn-sm">
          Back to customers
        </Link>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <div className="d-flex flex-wrap gap-2 mb-3">
                <span className={`badge ${customer.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>
                  {customer.status}
                </span>
                {customer.sourceLeadId && (
                  <span className="badge bg-info text-dark">From lead</span>
                )}
              </div>
              <dl className="row mb-0">
                <dt className="col-sm-3">Email</dt>
                <dd className="col-sm-9">{customer.email}</dd>
                <dt className="col-sm-3">Phone</dt>
                <dd className="col-sm-9">{customer.phone}</dd>
                <dt className="col-sm-3">City</dt>
                <dd className="col-sm-9">{customer.city}</dd>
                <dt className="col-sm-3">Billing address</dt>
                <dd className="col-sm-9">{customer.billingAddress}</dd>
                {customer.portalUserId && (
                  <>
                    <dt className="col-sm-3">Portal user</dt>
                    <dd className="col-sm-9">{customer.portalUserId}</dd>
                  </>
                )}
              </dl>
            </div>
          </div>

          {nextStatus && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-header bg-white fw-semibold">Status</div>
              <div className="card-body">
                <button
                  type="button"
                  className={`btn btn-sm ${nextStatus === 'Inactive' ? 'btn-outline-danger' : 'btn-outline-success'}`}
                  disabled={statusMutation.isPending}
                  onClick={() => statusMutation.mutate(nextStatus)}
                >
                  Mark {nextStatus}
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
