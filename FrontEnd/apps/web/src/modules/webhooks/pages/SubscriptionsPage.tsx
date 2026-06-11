import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { webhooksApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { webhookSubscriptionRoute } from '@/core/router/routeMap';
import { useWebhookPermissions } from '../hooks/useWebhookPermissions';
import { EnabledBadge } from '../components/StatusBadge';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { SecretRevealModal } from '../components/SecretRevealModal';
import { formatUtc } from '../utils/format';
import type { WebhookSubscription } from '../types';

export default function SubscriptionsPage() {
  const toast = useToast();
  const queryClient = useQueryClient();
  const { canManage } = useWebhookPermissions();
  const { extractMessage, extractCorrelationId } = useApiError();

  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState<WebhookSubscription | null>(null);
  const [disableTarget, setDisableTarget] = useState<WebhookSubscription | null>(null);
  const [rotateTarget, setRotateTarget] = useState<WebhookSubscription | null>(null);
  const [revealedSecret, setRevealedSecret] = useState<string | null>(null);
  const [formName, setFormName] = useState('');
  const [formUrl, setFormUrl] = useState('');
  const [formEnabled, setFormEnabled] = useState(true);
  const [formEvents, setFormEvents] = useState('');

  const { data = [], isLoading, error } = useQuery({
    queryKey: ['webhooks', 'subscriptions'],
    queryFn: webhooksApi.listSubscriptions,
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['webhooks', 'subscriptions'] });

  const createMutation = useMutation({
    mutationFn: () =>
      webhooksApi.createSubscription({
        name: formName.trim(),
        endpointUrl: formUrl.trim(),
        subscribedEventNames: parseEvents(formEvents),
      }),
    onSuccess: (result) => {
      invalidate();
      setShowForm(false);
      setRevealedSecret(result.secret);
      toast.success('Subscription created.');
    },
  });

  const updateMutation = useMutation({
    mutationFn: () =>
      webhooksApi.updateSubscription(editing!.id, {
        name: formName.trim(),
        endpointUrl: formUrl.trim(),
        enabled: formEnabled,
        subscribedEventNames: parseEvents(formEvents),
      }),
    onSuccess: () => {
      invalidate();
      setShowForm(false);
      setEditing(null);
      toast.success('Subscription updated.');
    },
  });

  const disableMutation = useMutation({
    mutationFn: (id: string) => webhooksApi.disableSubscription(id),
    onSuccess: () => {
      invalidate();
      setDisableTarget(null);
      toast.success('Subscription disabled.');
    },
  });

  const rotateMutation = useMutation({
    mutationFn: (id: string) => webhooksApi.rotateSecret(id),
    onSuccess: (result) => {
      invalidate();
      setRotateTarget(null);
      setRevealedSecret(result.secret);
      toast.success('Secret rotated. Copy the new secret now.');
    },
  });

  function openCreate() {
    setEditing(null);
    setFormName('');
    setFormUrl('');
    setFormEnabled(true);
    setFormEvents('');
    setShowForm(true);
  }

  function openEdit(sub: WebhookSubscription) {
    setEditing(sub);
    setFormName(sub.name);
    setFormUrl(sub.endpointUrl);
    setFormEnabled(sub.enabled);
    setFormEvents('');
    setShowForm(true);
  }

  function parseEvents(value: string): string[] | undefined {
    const items = value.split(',').map((s) => s.trim()).filter(Boolean);
    return items.length > 0 ? items : undefined;
  }

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

  return (
    <div>
      <PageHeader
        title="Webhook subscriptions"
        subtitle={`${data.length} subscription${data.length !== 1 ? 's' : ''}`}
      >
        {canManage && (
          <button type="button" className="btn btn-primary btn-sm" onClick={openCreate}>
            Create subscription
          </button>
        )}
      </PageHeader>

      {data.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState
              icon="link"
              title="No subscriptions"
              description="Create a subscription to deliver events to your endpoint."
            />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>Name</th>
                  <th>Endpoint</th>
                  <th>Status</th>
                  <th>Created</th>
                  <th>Updated</th>
                  <th aria-label="Actions" />
                </tr>
              </thead>
              <tbody>
                {data.map((sub) => (
                  <tr key={sub.id}>
                    <td className="fw-semibold">{sub.name}</td>
                    <td className="small text-break">{sub.endpointUrl}</td>
                    <td><EnabledBadge enabled={sub.enabled} /></td>
                    <td className="small">{formatUtc(sub.createdOnUtc)}</td>
                    <td className="small">{formatUtc(sub.updatedOnUtc)}</td>
                    <td className="text-end">
                      <Link
                        to={webhookSubscriptionRoute(sub.id)}
                        className="btn btn-sm btn-outline-secondary me-1"
                      >
                        Details
                      </Link>
                      {canManage && (
                        <>
                          <button type="button" className="btn btn-sm btn-outline-primary me-1" onClick={() => openEdit(sub)}>
                            Edit
                          </button>
                          <button type="button" className="btn btn-sm btn-outline-warning me-1" onClick={() => setRotateTarget(sub)}>
                            Rotate secret
                          </button>
                          {sub.enabled && (
                            <button type="button" className="btn btn-sm btn-outline-danger" onClick={() => setDisableTarget(sub)}>
                              Disable
                            </button>
                          )}
                        </>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      <Modal show={showForm} onHide={() => setShowForm(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>{editing ? 'Edit subscription' : 'Create subscription'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="mb-3">
            <label className="form-label">Name</label>
            <input className="form-control" value={formName} onChange={(e) => setFormName(e.target.value)} />
          </div>
          <div className="mb-3">
            <label className="form-label">Endpoint URL</label>
            <input className="form-control" value={formUrl} onChange={(e) => setFormUrl(e.target.value)} />
          </div>
          <div className="mb-3">
            <label className="form-label">Subscribed events (comma-separated, empty = all)</label>
            <input className="form-control" value={formEvents} onChange={(e) => setFormEvents(e.target.value)} placeholder="user.created, user.updated" />
          </div>
          {editing && (
            <div className="form-check">
              <input
                className="form-check-input"
                type="checkbox"
                checked={formEnabled}
                onChange={(e) => setFormEnabled(e.target.checked)}
                id="sub-enabled"
              />
              <label className="form-check-label" htmlFor="sub-enabled">Enabled</label>
            </div>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={() => setShowForm(false)}>Cancel</Button>
          <Button
            variant="primary"
            disabled={!formName.trim() || !formUrl.trim() || createMutation.isPending || updateMutation.isPending}
            onClick={() => (editing ? updateMutation.mutate() : createMutation.mutate())}
          >
            {editing ? 'Save' : 'Create'}
          </Button>
        </Modal.Footer>
      </Modal>

      <ConfirmDialog
        show={!!disableTarget}
        title="Disable subscription"
        message={`Disable "${disableTarget?.name}"? No further deliveries will be sent.`}
        confirmLabel="Disable"
        confirmVariant="danger"
        isLoading={disableMutation.isPending}
        onConfirm={() => disableTarget && disableMutation.mutate(disableTarget.id)}
        onCancel={() => setDisableTarget(null)}
      />

      <ConfirmDialog
        show={!!rotateTarget}
        title="Rotate signing secret"
        message={`Rotate the secret for "${rotateTarget?.name}"? Update your subscriber before the old secret stops working.`}
        confirmLabel="Rotate"
        confirmVariant="warning"
        isLoading={rotateMutation.isPending}
        onConfirm={() => rotateTarget && rotateMutation.mutate(rotateTarget.id)}
        onCancel={() => setRotateTarget(null)}
      />

      {revealedSecret && (
        <SecretRevealModal
          show
          secret={revealedSecret}
          onClose={() => setRevealedSecret(null)}
        />
      )}
    </div>
  );
}
