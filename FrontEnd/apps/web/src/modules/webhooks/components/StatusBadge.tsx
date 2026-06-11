import type { WebhookDeliveryStatus } from '../types';

const STATUS_CLASS: Record<WebhookDeliveryStatus, string> = {
  Pending: 'bg-secondary',
  Succeeded: 'bg-success',
  Failed: 'bg-danger',
  Retrying: 'bg-warning text-dark',
  DeadLettered: 'bg-dark',
};

export function DeliveryStatusBadge({ status }: { status: WebhookDeliveryStatus }) {
  return <span className={`badge ${STATUS_CLASS[status] ?? 'bg-secondary'}`}>{status}</span>;
}

export function EnabledBadge({ enabled }: { enabled: boolean }) {
  return (
    <span className={`badge ${enabled ? 'bg-success' : 'bg-secondary'}`}>
      {enabled ? 'Enabled' : 'Disabled'}
    </span>
  );
}
