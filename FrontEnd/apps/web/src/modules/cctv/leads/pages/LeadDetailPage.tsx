import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { leadsApi } from '../api';
import type { LeadStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

const NEXT_STATUS: Partial<Record<LeadStatus, LeadStatus[]>> = {
  New: ['Contacted', 'Lost'],
  Contacted: ['Qualified', 'Lost'],
  Qualified: ['QuotationSent', 'Lost'],
  QuotationSent: ['Negotiation', 'Lost'],
  Negotiation: ['Won', 'Lost'],
};

export default function LeadDetailPage() {
  const { leadId = '' } = useParams<{ leadId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();
  const [remarkText, setRemarkText] = useState('');

  const leadQuery = useQuery({
    queryKey: ['cctv', 'leads', leadId],
    queryFn: () => leadsApi.get(leadId),
    enabled: Boolean(leadId),
  });

  const activitiesQuery = useQuery({
    queryKey: ['cctv', 'leads', leadId, 'activities'],
    queryFn: () => leadsApi.getActivities(leadId),
    enabled: Boolean(leadId),
  });

  const remarksQuery = useQuery({
    queryKey: ['cctv', 'leads', leadId, 'remarks'],
    queryFn: () => leadsApi.getRemarks(leadId),
    enabled: Boolean(leadId),
  });

  const statusMutation = useMutation({
    mutationFn: (toStatus: LeadStatus) =>
      leadsApi.changeStatus(leadId, {
        toStatus,
        rowVersion: leadQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Lead status updated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'leads'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const remarkMutation = useMutation({
    mutationFn: () => leadsApi.addRemark(leadId, { text: remarkText.trim() }),
    onSuccess: () => {
      setRemarkText('');
      toast.success('Remark added');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'leads', leadId] });
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'leads', leadId, 'remarks'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (leadQuery.isLoading) return <Spinner fullPage />;

  if (leadQuery.error) {
    return <AlertMessage message={extractMessage(leadQuery.error)} variant="danger" />;
  }

  const lead = leadQuery.data;
  if (!lead) return null;

  const transitions = NEXT_STATUS[lead.status] ?? [];
  const isTerminal = lead.status === 'Lost' || lead.status === 'Converted';

  return (
    <div>
      <PageHeader title={lead.contactName} subtitle={lead.leadNumber}>
        <Link to={ROUTES.cctv.admin.leads} className="btn btn-outline-secondary btn-sm">
          Back to pipeline
        </Link>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <div className="d-flex flex-wrap gap-2 mb-3">
                <span className="badge bg-primary">{lead.status}</span>
                <span className="badge bg-secondary">{lead.source}</span>
              </div>
              <dl className="row mb-0">
                <dt className="col-sm-3">Email</dt>
                <dd className="col-sm-9">{lead.email}</dd>
                <dt className="col-sm-3">Phone</dt>
                <dd className="col-sm-9">{lead.phone}</dd>
                <dt className="col-sm-3">City</dt>
                <dd className="col-sm-9">{lead.city}</dd>
                {lead.organizationName && (
                  <>
                    <dt className="col-sm-3">Organization</dt>
                    <dd className="col-sm-9">{lead.organizationName}</dd>
                  </>
                )}
                {lead.requirementSummary && (
                  <>
                    <dt className="col-sm-3">Requirement</dt>
                    <dd className="col-sm-9">{lead.requirementSummary}</dd>
                  </>
                )}
              </dl>
            </div>
          </div>

          {!isTerminal && transitions.length > 0 && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-header bg-white fw-semibold">Status transitions</div>
              <div className="card-body d-flex flex-wrap gap-2">
                {transitions.map((status) => (
                  <button
                    key={status}
                    type="button"
                    className={`btn btn-sm ${status === 'Lost' ? 'btn-outline-danger' : 'btn-outline-primary'}`}
                    disabled={statusMutation.isPending}
                    onClick={() => statusMutation.mutate(status)}
                  >
                    Mark {status}
                  </button>
                ))}
              </div>
            </div>
          )}

          {lead.status === 'Won' && (
            <AlertMessage
              variant="info"
              icon="info-circle"
              message="This lead is Won. Use the conversion wizard (B3) to create Customer + Site + AMC Contract."
            />
          )}

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white fw-semibold">Activity timeline</div>
            <div className="card-body">
              {(activitiesQuery.data ?? []).length === 0 ? (
                <p className="text-muted mb-0">No activities recorded yet.</p>
              ) : (
                <ul className="list-unstyled mb-0">
                  {(activitiesQuery.data ?? []).map((activity) => (
                    <li key={activity.id} className="border-bottom py-2">
                      <div className="small text-muted">
                        {new Date(activity.occurredAtUtc).toLocaleString()} · {activity.activityType}
                      </div>
                      <div>{activity.description}</div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white fw-semibold">Remarks</div>
            <div className="card-body">
              {!isTerminal && (
                <div className="mb-3">
                  <textarea
                    className="form-control mb-2"
                    rows={3}
                    placeholder="Add a remark…"
                    value={remarkText}
                    onChange={(e) => setRemarkText(e.target.value)}
                    aria-label="Remark text"
                  />
                  <button
                    type="button"
                    className="btn btn-primary btn-sm"
                    disabled={!remarkText.trim() || remarkMutation.isPending}
                    onClick={() => remarkMutation.mutate()}
                  >
                    Add remark
                  </button>
                </div>
              )}
              {(remarksQuery.data ?? []).length === 0 ? (
                <p className="text-muted mb-0">No remarks yet.</p>
              ) : (
                <ul className="list-unstyled mb-0">
                  {(remarksQuery.data ?? []).map((remark) => (
                    <li key={remark.id} className="border-bottom py-2">
                      <div className="small text-muted">
                        {new Date(remark.createdAtUtc).toLocaleString()}
                      </div>
                      <div>{remark.text}</div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
