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
import { ConfirmDialog } from '../components/ConfirmDialog';
import { formatUtc, tryFormatJson } from '../utils/format';

function classifyFailure(code: number | null, reason: string | null): string {
  if (code && [400, 401, 403, 404, 410, 422].includes(code)) return 'Permanent';
  if (code && [408, 429, 500, 502, 503, 504].includes(code)) return 'Transient (exhausted)';
  if (reason?.toLowerCase().includes('timeout') || reason?.toLowerCase().includes('connection')) {
    return 'Transient (exhausted)';
  }
  return 'Unknown';
}

export default function DeadLetterDetailPage() {
  const { id = '' } = useParams();
  const navigate = useNavigate();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { canManage } = useWebhookPermissions();
  const { extractMessage, extractCorrelationId } = useApiError();
  const [showReplay, setShowReplay] = useState(false);

  const { data, isLoading, error } = useQuery({
    queryKey: ['webhooks', 'deadletters', id],
    queryFn: () => webhooksApi.getDeadLetter(id),
    enabled: !!id,
  });

  const replayMutation = useMutation({
    mutationFn: () => webhooksApi.replayDeadLetter(id),
    onSuccess: (delivery) => {
      void queryClient.invalidateQueries({ queryKey: ['webhooks'] });
      toast.success('Dead letter replayed. New delivery created.');
      setShowReplay(false);
      navigate(webhookDeliveryRoute(delivery.id));
    },
  });

  if (isLoading) return <Spinner fullPage />;

  if (error || !data) {
    const correlationId = error ? extractCorrelationId(error) : null;
    return (
      <div>
        <AlertMessage message={error ? extractMessage(error) : 'Dead letter not found.'} variant="danger" />
        {correlationId && <CorrelationIdCopy correlationId={correlationId} className="mt-2" />}
      </div>
    );
  }

  return (
    <div>
      <PageHeader title={`Dead letter: ${data.eventName}`} subtitle={data.id}>
        {canManage && (
          <button type="button" className="btn btn-sm btn-primary" onClick={() => setShowReplay(true)}>
            Replay
          </button>
        )}
        <Link to={ROUTES.webhooks.deadLetters} className="btn btn-sm btn-outline-secondary">Back</Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          <dl className="row small mb-0">
            <dt className="col-sm-3">Original delivery</dt>
            <dd className="col-sm-9">
              <Link to={webhookDeliveryRoute(data.deliveryId)}>{data.deliveryId}</Link>
            </dd>
            <dt className="col-sm-3">Failure code</dt>
            <dd className="col-sm-9">{data.failureCode ?? '—'}</dd>
            <dt className="col-sm-3">Failure reason</dt>
            <dd className="col-sm-9">{data.failureReason ?? '—'}</dd>
            <dt className="col-sm-3">Classification</dt>
            <dd className="col-sm-9">{classifyFailure(data.failureCode, data.failureReason)}</dd>
            <dt className="col-sm-3">Retry count</dt>
            <dd className="col-sm-9">{data.retryCount}</dd>
            <dt className="col-sm-3">Created</dt>
            <dd className="col-sm-9">{formatUtc(data.createdOnUtc)}</dd>
            <dt className="col-sm-3">Correlation</dt>
            <dd className="col-sm-9">
              {data.correlationId
                ? <CorrelationIdCopy correlationId={data.correlationId} />
                : '—'}
            </dd>
          </dl>
        </div>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="card-header bg-white fw-semibold">Payload</div>
        <div className="card-body">
          <pre className="small bg-light p-3 rounded mb-0">{tryFormatJson(data.payload)}</pre>
        </div>
      </div>

      <ConfirmDialog
        show={showReplay}
        title="Replay dead letter"
        message="Create a new delivery attempt with the same payload and correlation ID?"
        confirmLabel="Replay"
        isLoading={replayMutation.isPending}
        onConfirm={() => replayMutation.mutate()}
        onCancel={() => setShowReplay(false)}
      />
    </div>
  );
}
