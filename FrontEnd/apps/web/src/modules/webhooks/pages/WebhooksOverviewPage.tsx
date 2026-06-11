import { Link } from 'react-router-dom';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';
import { useWebhookMetrics } from '../hooks/useWebhookMetrics';
import { formatDuration, formatPercent } from '../utils/format';

export default function WebhooksOverviewPage() {
  const { extractMessage, extractCorrelationId } = useApiError();
  const { metrics, isLoading, error } = useWebhookMetrics();

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    const correlationId = extractCorrelationId(error);
    return (
      <div>
        <AlertMessage message={extractMessage(error)} variant="danger" />
        {correlationId && <CorrelationIdCopy correlationId={correlationId} className="mt-2" />}
      </div>
    );
  }

  const cards = metrics
    ? [
        { label: 'Total deliveries (sample)', value: metrics.totalDeliveries },
        { label: 'Success rate', value: formatPercent(metrics.successRate) },
        { label: 'Failure rate', value: formatPercent(metrics.failureRate) },
        { label: 'Retry count', value: metrics.retryCount },
        { label: 'Dead letters', value: metrics.deadLetterCount },
        { label: 'Avg delivery time', value: formatDuration(metrics.averageDeliveryMs) },
      ]
    : [];

  return (
    <div>
      <PageHeader
        title="Webhook Operations Center"
        subtitle="Monitor delivery health, investigate failures, and manage subscriptions."
      />

      <div className="row g-3 mb-4">
        {cards.map((card) => (
          <div key={card.label} className="col-sm-6 col-xl-4">
            <div className="card border-0 shadow-sm h-100">
              <div className="card-body">
                <div className="text-muted small">{card.label}</div>
                <div className="fs-4 fw-semibold">{card.value}</div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {!metrics && (
        <AlertMessage
          message="No delivery data yet. Metrics are derived from recent delivery and dead letter history."
          variant="info"
        />
      )}

      <div className="row g-3">
        <div className="col-md-4">
          <Link to={ROUTES.webhooks.subscriptions} className="card border-0 shadow-sm text-decoration-none h-100">
            <div className="card-body">
              <h5 className="card-title">Subscriptions</h5>
              <p className="card-text text-muted small mb-0">Create and manage webhook endpoints.</p>
            </div>
          </Link>
        </div>
        <div className="col-md-4">
          <Link to={ROUTES.webhooks.deliveries} className="card border-0 shadow-sm text-decoration-none h-100">
            <div className="card-body">
              <h5 className="card-title">Delivery history</h5>
              <p className="card-text text-muted small mb-0">Inspect outcomes, correlation IDs, and responses.</p>
            </div>
          </Link>
        </div>
        <div className="col-md-4">
          <Link to={ROUTES.webhooks.deadLetters} className="card border-0 shadow-sm text-decoration-none h-100">
            <div className="card-body">
              <h5 className="card-title">Dead letter queue</h5>
              <p className="card-text text-muted small mb-0">Review exhausted deliveries and replay when ready.</p>
            </div>
          </Link>
        </div>
      </div>
    </div>
  );
}
