import { Link, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { webhooksApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES, webhookDeliveryRoute } from '@/core/router/routeMap';
import { EnabledBadge, DeliveryStatusBadge } from '../components/StatusBadge';
import { formatUtc } from '../utils/format';

export default function SubscriptionDetailPage() {
  const { id = '' } = useParams();
  const { extractMessage, extractCorrelationId } = useApiError();

  const subscriptionQuery = useQuery({
    queryKey: ['webhooks', 'subscriptions', id],
    queryFn: () => webhooksApi.getSubscription(id),
    enabled: !!id,
  });

  const deliveriesQuery = useQuery({
    queryKey: ['webhooks', 'deliveries', { subscriptionId: id, limit: 10 }],
    queryFn: () => webhooksApi.listDeliveries({ subscriptionId: id, limit: 10 }),
    enabled: !!id,
  });

  if (subscriptionQuery.isLoading) return <Spinner fullPage />;

  if (subscriptionQuery.error) {
    const correlationId = extractCorrelationId(subscriptionQuery.error);
    return (
      <div>
        <AlertMessage message={extractMessage(subscriptionQuery.error)} variant="danger" />
        {correlationId && <CorrelationIdCopy correlationId={correlationId} className="mt-2" />}
      </div>
    );
  }

  const sub = subscriptionQuery.data!;
  const deliveries = deliveriesQuery.data ?? [];
  const failures = deliveries.filter((d) => d.status === 'Failed' || d.status === 'DeadLettered');

  return (
    <div>
      <PageHeader title={sub.name} subtitle="Subscription details">
        <Link to={ROUTES.webhooks.subscriptions} className="btn btn-sm btn-outline-secondary">
          Back to subscriptions
        </Link>
      </PageHeader>

      <div className="row g-3 mb-4">
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h6 className="text-muted">Metadata</h6>
              <dl className="row small mb-0">
                <dt className="col-sm-4">Endpoint</dt>
                <dd className="col-sm-8 text-break">{sub.endpointUrl}</dd>
                <dt className="col-sm-4">Status</dt>
                <dd className="col-sm-8"><EnabledBadge enabled={sub.enabled} /></dd>
                <dt className="col-sm-4">Created</dt>
                <dd className="col-sm-8">{formatUtc(sub.createdOnUtc)}</dd>
                <dt className="col-sm-4">Updated</dt>
                <dd className="col-sm-8">{formatUtc(sub.updatedOnUtc)}</dd>
              </dl>
            </div>
          </div>
        </div>
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h6 className="text-muted">Secret information</h6>
              <p className="small text-muted mb-0">
                Signing secrets are never displayed after creation. Use <strong>Rotate secret</strong> on the
                subscriptions list to issue a new secret (shown once).
              </p>
            </div>
          </div>
        </div>
      </div>

      <div className="row g-3">
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white fw-semibold">Recent deliveries</div>
            <div className="list-group list-group-flush">
              {deliveries.length === 0 && (
                <div className="list-group-item text-muted small">No deliveries yet.</div>
              )}
              {deliveries.map((d) => (
                <Link
                  key={d.id}
                  to={webhookDeliveryRoute(d.id)}
                  className="list-group-item list-group-item-action d-flex justify-content-between align-items-center"
                >
                  <span className="small">{d.eventName}</span>
                  <DeliveryStatusBadge status={d.status} />
                </Link>
              ))}
            </div>
          </div>
        </div>
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white fw-semibold">Recent failures</div>
            <div className="list-group list-group-flush">
              {failures.length === 0 && (
                <div className="list-group-item text-muted small">No recent failures.</div>
              )}
              {failures.map((d) => (
                <Link
                  key={d.id}
                  to={webhookDeliveryRoute(d.id)}
                  className="list-group-item list-group-item-action"
                >
                  <div className="small fw-semibold">{d.eventName}</div>
                  <div className="small text-muted">{d.lastFailureReason ?? d.responseBody ?? 'Failed'}</div>
                </Link>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
