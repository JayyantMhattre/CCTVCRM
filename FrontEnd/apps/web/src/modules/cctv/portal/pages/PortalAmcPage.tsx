import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { portalApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';

export default function PortalAmcPage() {
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();
  const [renewalMessage, setRenewalMessage] = useState('');

  const amcQuery = useQuery({
    queryKey: ['cctv', 'portal', 'amc'],
    queryFn: () => portalApi.getAmc(),
  });

  const historyQuery = useQuery({
    queryKey: ['cctv', 'portal', 'amc', 'history', amcQuery.data?.contractId],
    queryFn: () => portalApi.getContractHistory(amcQuery.data!.contractId),
    enabled: Boolean(amcQuery.data?.contractId),
  });

  const renewalMutation = useMutation({
    mutationFn: () => portalApi.submitRenewalRequest(amcQuery.data!.contractId, renewalMessage.trim() || null),
    onSuccess: () => {
      toast.success('Renewal request submitted.');
      setRenewalMessage('');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'portal', 'amc'] });
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'portal', 'amc', 'history'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (amcQuery.isLoading) return <Spinner fullPage />;
  if (amcQuery.error) return <AlertMessage message={extractMessage(amcQuery.error)} variant="danger" />;

  const amc = amcQuery.data;

  if (!amc) {
    return (
      <div>
        <PageHeader title="My AMC" subtitle="Annual maintenance contract" />
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No active AMC" description="Contact Aarvii to activate an AMC for your site." />
          </div>
        </div>
      </div>
    );
  }

  const terms = historyQuery.data?.terms ?? [];
  const renewalPending = terms.some((t) => t.renewalRequestedByCustomer);

  return (
    <div>
      <PageHeader title="My AMC" subtitle={amc.contractNumber} />

      <div className="row g-3 mb-4">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <h2 className="h6 text-muted">Current plan</h2>
              <div className="h4 fw-semibold mb-1">{amc.planName}</div>
              <div className="text-muted small mb-3">{amc.planCode} · Term {amc.termNo}</div>
              <dl className="row mb-0 small">
                <dt className="col-sm-4">Coverage period</dt>
                <dd className="col-sm-8">
                  {amc.startDate} — {amc.endDate}
                </dd>
                <dt className="col-sm-4">Status</dt>
                <dd className="col-sm-8">{amc.status}</dd>
                <dt className="col-sm-4">Visit frequency</dt>
                <dd className="col-sm-8">{amc.visitFrequencyPerYear} visits / year</dd>
                <dt className="col-sm-4">Agreed price</dt>
                <dd className="col-sm-8">{amc.agreedPrice.toFixed(2)}</dd>
                <dt className="col-sm-4">SLA</dt>
                <dd className="col-sm-8">{amc.slaTerms}</dd>
              </dl>
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">Included services</h2>
            </div>
            <ul className="list-group list-group-flush">
              {amc.includedServices.map((service) => (
                <li key={service} className="list-group-item">
                  {service}
                </li>
              ))}
            </ul>
          </div>

          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">AMC history</h2>
            </div>
            <div className="table-responsive">
              <table className="table mb-0 align-middle">
                <thead className="table-light">
                  <tr>
                    <th>Term</th>
                    <th>Plan</th>
                    <th>Period</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {terms.map((term) => (
                    <tr key={term.id}>
                      <td>{term.termNo}</td>
                      <td>{term.planCode}</td>
                      <td className="small">
                        {term.startDate} — {term.endDate}
                      </td>
                      <td>{term.status}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <h2 className="h6 fw-semibold">Request renewal</h2>
              <p className="small text-muted">
                Submit a renewal request before your current term ends. Our team will prepare the next term.
              </p>
              {renewalPending && (
                <AlertMessage message="A renewal request is already on file." variant="info" />
              )}
              <textarea
                className="form-control mb-3"
                rows={3}
                placeholder="Optional message"
                value={renewalMessage}
                onChange={(e) => setRenewalMessage(e.target.value)}
                disabled={renewalPending || renewalMutation.isPending}
              />
              <button
                type="button"
                className="btn btn-primary w-100"
                disabled={renewalPending || renewalMutation.isPending}
                onClick={() => renewalMutation.mutate()}
              >
                {renewalMutation.isPending ? 'Submitting…' : 'Submit renewal request'}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
