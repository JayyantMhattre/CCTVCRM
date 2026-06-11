import { useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { webhooksApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES, webhookDeliveryRoute } from '@/core/router/routeMap';
import { useWebhookPermissions } from '../hooks/useWebhookPermissions';
import { DeliveryStatusBadge } from '../components/StatusBadge';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { deliveryDurationMs, formatDuration, formatUtc, tryFormatJson } from '../utils/format';

export default function DeliveryDetailPage() {
  const { id = '' } = useParams();
  const navigate = useNavigate();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { canManage } = useWebhookPermissions();
  const { extractMessage, extractCorrelationId } = useApiError();
  const [showRetry, setShowRetry] = useState(false);

  const { data, isLoading, error } = useQuery({
    queryKey: ['webhooks', 'deliveries', id],
    queryFn: () => webhooksApi.getDelivery(id),
    enabled: !!id,
  });

  const retryMutation = useMutation({
    mutationFn: () => webhooksApi.retryDelivery(id),
    onSuccess: (delivery) => {
      void queryClient.invalidateQueries({ queryKey: ['webhooks', 'deliveries'] });
      toast.success('Delivery queued for retry.');
      setShowRetry(false);
      navigate(webhookDeliveryRoute(delivery.id));
    },
  });

  if (isLoading) return <Spinner fullPage />;

  if (error || !data) {
    const correlationId = error ? extractCorrelationId(error) : null;
    return (
      <div>
        <AlertMessage message={error ? extractMessage(error) : 'Delivery not found.'} variant="danger" />
        {correlationId && <CorrelationIdCopy correlationId={correlationId} className="mt-2" />}
      </div>
    );
  }

  const duration = deliveryDurationMs(data.startedOnUtc, data.completedOnUtc);
  const canRetry = canManage && (data.status === 'Failed' || data.status === 'Retrying');

  return (
    <div>
      <PageHeader title={`Delivery ${data.eventName}`} subtitle={data.id}>
        {canRetry && (
          <button type="button" className="btn btn-sm btn-warning" onClick={() => setShowRetry(true)}>
            Retry now
          </button>
        )}
        <Link to={ROUTES.webhooks.deliveries} className="btn btn-sm btn-outline-secondary">Back</Link>
      </PageHeader>

      <div className="row g-3 mb-3">
        <div className="col-md-8">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <dl className="row small mb-0">
                <dt className="col-sm-3">Status</dt>
                <dd className="col-sm-9"><DeliveryStatusBadge status={data.status} /></dd>
                <dt className="col-sm-3">Response code</dt>
                <dd className="col-sm-9">{data.responseCode ?? '—'}</dd>
                <dt className="col-sm-3">Duration</dt>
                <dd className="col-sm-9">{formatDuration(duration)}</dd>
                <dt className="col-sm-3">Attempt</dt>
                <dd className="col-sm-9">{data.attemptNumber} (retries: {data.retryCount})</dd>
                <dt className="col-sm-3">Started</dt>
                <dd className="col-sm-9">{formatUtc(data.startedOnUtc)}</dd>
                <dt className="col-sm-3">Completed</dt>
                <dd className="col-sm-9">{formatUtc(data.completedOnUtc)}</dd>
                <dt className="col-sm-3">Next retry</dt>
                <dd className="col-sm-9">{formatUtc(data.nextRetryOnUtc)}</dd>
                <dt className="col-sm-3">Correlation</dt>
                <dd className="col-sm-9">
                  {data.correlationId
                    ? <CorrelationIdCopy correlationId={data.correlationId} />
                    : '—'}
                </dd>
              </dl>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card border-0 shadow-sm">
            <div className="card-body small">
              <h6 className="text-muted">Outbound headers (standard)</h6>
              <ul className="mb-0 ps-3">
                <li>X-Webhook-Event: {data.eventName}</li>
                <li>X-Webhook-Version: {data.eventVersion}</li>
                <li>X-Webhook-Delivery-Id: {data.id}</li>
                {data.correlationId && <li>X-Correlation-Id: {data.correlationId}</li>}
                <li>X-Webhook-Signature: (HMAC-SHA256)</li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-header bg-white fw-semibold">Response body</div>
        <div className="card-body">
          <pre className="small bg-light p-3 rounded mb-0">{tryFormatJson(data.responseBody) || '—'}</pre>
        </div>
      </div>

      {(data.lastFailureReason || data.lastFailureCode) && (
        <div className="card border-0 shadow-sm">
          <div className="card-header bg-white fw-semibold">Failure details</div>
          <div className="card-body small">
            <div>Code: {data.lastFailureCode ?? '—'}</div>
            <div>Reason: {data.lastFailureReason ?? '—'}</div>
          </div>
        </div>
      )}

      <ConfirmDialog
        show={showRetry}
        title="Retry delivery"
        message="Queue this delivery for an immediate retry attempt?"
        confirmLabel="Retry"
        confirmVariant="warning"
        isLoading={retryMutation.isPending}
        onConfirm={() => retryMutation.mutate()}
        onCancel={() => setShowRetry(false)}
      />
    </div>
  );
}
