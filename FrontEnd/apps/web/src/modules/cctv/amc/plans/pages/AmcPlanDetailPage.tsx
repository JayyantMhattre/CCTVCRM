import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { amcPlansApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

export default function AmcPlanDetailPage() {
  const { planId = '' } = useParams<{ planId: string }>();
  const { extractMessage } = useApiError();

  const { data: plan, isLoading, error } = useQuery({
    queryKey: ['cctv', 'amc-plans', planId],
    queryFn: () => amcPlansApi.get(planId),
    enabled: Boolean(planId),
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;
  if (!plan) return null;

  return (
    <div>
      <PageHeader title={plan.name} subtitle={plan.planCode}>
        <Link to={ROUTES.cctv.admin.amcPlans} className="btn btn-outline-secondary btn-sm">
          Back to plans
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          <dl className="row mb-0">
            <dt className="col-sm-3">Status</dt>
            <dd className="col-sm-9"><span className={`badge ${plan.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>{plan.status}</span></dd>
            <dt className="col-sm-3">Description</dt>
            <dd className="col-sm-9">{plan.description ?? '—'}</dd>
            <dt className="col-sm-3">Row version</dt>
            <dd className="col-sm-9">{plan.rowVersion}</dd>
          </dl>
        </div>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="card-header bg-white"><strong>Versions</strong></div>
        <div className="table-responsive">
          <table className="table table-sm mb-0">
            <thead>
              <tr>
                <th>#</th>
                <th>Price</th>
                <th>Visits/yr</th>
                <th>Effective from</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {plan.versions.map((v) => (
                <tr key={v.id}>
                  <td>v{v.versionNo}</td>
                  <td>{v.price.toLocaleString()}</td>
                  <td>{v.visitFrequencyPerYear}</td>
                  <td>{v.effectiveFrom}</td>
                  <td><span className="badge bg-light text-dark">{v.status}</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
