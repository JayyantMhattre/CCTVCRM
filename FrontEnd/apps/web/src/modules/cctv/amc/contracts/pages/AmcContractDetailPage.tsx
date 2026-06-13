import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { amcContractsApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

export default function AmcContractDetailPage() {
  const { contractId = '' } = useParams<{ contractId: string }>();
  const { extractMessage } = useApiError();

  const { data: contract, isLoading, error } = useQuery({
    queryKey: ['cctv', 'amc-contracts', contractId],
    queryFn: () => amcContractsApi.get(contractId),
    enabled: Boolean(contractId),
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;
  if (!contract) return null;

  return (
    <div>
      <PageHeader title={contract.contractNumber} subtitle={`Site ${contract.siteId.slice(0, 8)}…`}>
        <Link to={ROUTES.cctv.admin.amcContracts} className="btn btn-outline-secondary btn-sm">Back to contracts</Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          <dl className="row mb-0">
            <dt className="col-sm-3">Status</dt>
            <dd className="col-sm-9"><span className={`badge ${contract.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>{contract.status}</span></dd>
            <dt className="col-sm-3">Customer</dt>
            <dd className="col-sm-9"><code>{contract.customerId}</code></dd>
            <dt className="col-sm-3">Site</dt>
            <dd className="col-sm-9"><code>{contract.siteId}</code></dd>
          </dl>
        </div>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="card-header bg-white"><strong>Term history</strong></div>
        <div className="table-responsive">
          <table className="table table-sm mb-0">
            <thead>
              <tr>
                <th>Term</th>
                <th>Plan</th>
                <th>Period</th>
                <th>Price</th>
                <th>Status</th>
                <th>Origin</th>
              </tr>
            </thead>
            <tbody>
              {contract.terms.map((t) => (
                <tr key={t.id}>
                  <td>#{t.termNo}</td>
                  <td>{t.planCode} v{t.planVersionNo}</td>
                  <td>{t.startDate} → {t.endDate}</td>
                  <td>{t.agreedPrice.toLocaleString()}</td>
                  <td><span className="badge bg-light text-dark">{t.status}</span></td>
                  <td>{t.origin}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
