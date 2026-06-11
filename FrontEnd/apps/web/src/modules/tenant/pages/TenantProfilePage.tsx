/**
 * TenantProfilePage — displays the current tenant's public profile.
 *
 * Uses TanStack Query to fetch and cache the tenant data.
 * The query key includes `tenantId` so it auto-invalidates on tenant change.
 */

import { useQuery }    from '@tanstack/react-query';
import { tenantApi }   from '../api';
import { PageHeader }  from '@/shared/components/PageHeader';
import { Spinner }     from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { format }      from 'date-fns';

export default function TenantProfilePage() {
  const { extractMessage } = useApiError();

  const { data: tenant, isLoading, error } = useQuery({
    queryKey: ['tenant', 'current'],
    queryFn:  () => tenantApi.getCurrent(),
    staleTime: 60_000, // Tenant profile changes infrequently.
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return (
      <AlertMessage
        message={extractMessage(error)}
        variant="danger"
        icon="exclamation-circle"
      />
    );
  }

  if (!tenant) return null;

  return (
    <div>
      <PageHeader title="Tenant Profile" subtitle="Your workspace details." />

      <div className="card border-0 shadow-sm" style={{ maxWidth: '600px' }}>
        <div className="card-body">
          <dl className="row mb-0">
            <dt className="col-sm-4 text-muted">Name</dt>
            <dd className="col-sm-8 fw-semibold">{tenant.name}</dd>

            <dt className="col-sm-4 text-muted">Slug</dt>
            <dd className="col-sm-8 font-monospace">{tenant.slug}</dd>

            <dt className="col-sm-4 text-muted">Plan</dt>
            <dd className="col-sm-8">
              <span className="badge bg-primary">{tenant.plan}</span>
            </dd>

            <dt className="col-sm-4 text-muted">Status</dt>
            <dd className="col-sm-8">
              <span
                className={`badge ${
                  tenant.status === 'Active'    ? 'bg-success'
                  : tenant.status === 'Suspended' ? 'bg-warning text-dark'
                  :                                 'bg-secondary'
                }`}
              >
                {tenant.status}
              </span>
            </dd>

            {tenant.customDomain && (
              <>
                <dt className="col-sm-4 text-muted">Custom Domain</dt>
                <dd className="col-sm-8 font-monospace">{tenant.customDomain}</dd>
              </>
            )}

            <dt className="col-sm-4 text-muted">Created</dt>
            <dd className="col-sm-8">
              {format(new Date(tenant.createdOnUtc), 'PPP')}
            </dd>

            <dt className="col-sm-4 text-muted">Tenant ID</dt>
            <dd className="col-sm-8">
              <code className="text-muted small">{tenant.tenantId}</code>
            </dd>
          </dl>
        </div>
      </div>
    </div>
  );
}
