import { Link, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apikeysApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';
import { useApiKeyPermissions } from '../hooks/useApiKeyPermissions';
import { ConfirmDialog } from '@/modules/webhooks/components/ConfirmDialog';
import { KeyRevealModal } from '../components/KeyRevealModal';
import { formatUtc, formatPercent } from '@/modules/webhooks/utils/format';
import { useState } from 'react';
import type { ApiKey } from '../types';

function isExpired(key: ApiKey): boolean {
  if (!key.expiresOnUtc) return false;
  return new Date(key.expiresOnUtc) <= new Date();
}

function isActive(key: ApiKey): boolean {
  return key.enabled && !key.revokedOnUtc && !isExpired(key);
}

function KeyStatusBadge({ apiKey }: { apiKey: ApiKey }) {
  if (apiKey.revokedOnUtc) {
    return <span className="badge bg-danger">Revoked</span>;
  }
  if (!apiKey.enabled) {
    return <span className="badge bg-secondary">Disabled</span>;
  }
  if (isExpired(apiKey)) {
    return <span className="badge bg-warning text-dark">Expired</span>;
  }
  return <span className="badge bg-success">Active</span>;
}

export default function ApiKeyDetailPage() {
  const { id = '' } = useParams();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { canManage } = useApiKeyPermissions();
  const { extractMessage, extractCorrelationId } = useApiError();

  const [rotateOpen, setRotateOpen] = useState(false);
  const [revokeOpen, setRevokeOpen] = useState(false);
  const [revealedKey, setRevealedKey] = useState<string | null>(null);

  const { data: apiKey, isLoading, error } = useQuery({
    queryKey: ['apikeys', 'detail', id],
    queryFn: () => apikeysApi.get(id),
    enabled: !!id,
  });

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['apikeys'] });

  const rotateMutation = useMutation({
    mutationFn: () => apikeysApi.rotate(id),
    onSuccess: (result) => {
      invalidate();
      setRotateOpen(false);
      setRevealedKey(result.plaintextKey);
      toast.success('API key rotated. Copy the new key now.');
    },
  });

  const revokeMutation = useMutation({
    mutationFn: () => apikeysApi.revoke(id),
    onSuccess: () => {
      invalidate();
      setRevokeOpen(false);
      toast.success('API key revoked.');
    },
  });

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

  const key = apiKey!;
  const successRate = key.requestCount > 0 ? key.successCount / key.requestCount : 0;
  const failureRate = key.requestCount > 0 ? key.failureCount / key.requestCount : 0;
  const active = isActive(key);

  return (
    <div>
      <PageHeader title={key.name} subtitle="API key details">
        <div className="d-flex gap-2">
          <Link to={ROUTES.apikeys.list} className="btn btn-sm btn-outline-secondary">
            Back to API keys
          </Link>
          {canManage && active && (
            <>
              <button
                type="button"
                className="btn btn-sm btn-outline-warning"
                onClick={() => setRotateOpen(true)}
              >
                Rotate
              </button>
              <button
                type="button"
                className="btn btn-sm btn-outline-danger"
                onClick={() => setRevokeOpen(true)}
              >
                Revoke
              </button>
            </>
          )}
        </div>
      </PageHeader>

      <div className="row g-3 mb-4">
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h6 className="text-muted">Metadata</h6>
              <dl className="row small mb-0">
                <dt className="col-sm-4">Prefix</dt>
                <dd className="col-sm-8 font-monospace">{key.keyPrefix}…</dd>
                <dt className="col-sm-4">Environment</dt>
                <dd className="col-sm-8">{key.environment}</dd>
                <dt className="col-sm-4">Status</dt>
                <dd className="col-sm-8"><KeyStatusBadge apiKey={key} /></dd>
                <dt className="col-sm-4">Description</dt>
                <dd className="col-sm-8">{key.description || '—'}</dd>
                <dt className="col-sm-4">Created</dt>
                <dd className="col-sm-8">{formatUtc(key.createdOnUtc)}</dd>
                <dt className="col-sm-4">Expires</dt>
                <dd className="col-sm-8">{formatUtc(key.expiresOnUtc)}</dd>
                <dt className="col-sm-4">Last used</dt>
                <dd className="col-sm-8">{formatUtc(key.lastUsedOnUtc)}</dd>
                {key.revokedOnUtc && (
                  <>
                    <dt className="col-sm-4">Revoked</dt>
                    <dd className="col-sm-8">{formatUtc(key.revokedOnUtc)}</dd>
                  </>
                )}
              </dl>
            </div>
          </div>
        </div>
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h6 className="text-muted">Key information</h6>
              <p className="small text-muted mb-0">
                API keys are never displayed after creation. Use <strong>Rotate</strong> to issue a
                new key (shown once).
              </p>
            </div>
          </div>
        </div>
      </div>

      <div className="row g-3 mb-4">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white fw-semibold">Usage summary</div>
            <div className="card-body">
              <div className="row g-3 text-center">
                <div className="col-4">
                  <div className="fs-4 fw-semibold">{key.requestCount.toLocaleString()}</div>
                  <div className="small text-muted">Requests</div>
                </div>
                <div className="col-4">
                  <div className="fs-4 fw-semibold text-success">{key.successCount.toLocaleString()}</div>
                  <div className="small text-muted">Success ({formatPercent(successRate)})</div>
                </div>
                <div className="col-4">
                  <div className="fs-4 fw-semibold text-danger">{key.failureCount.toLocaleString()}</div>
                  <div className="small text-muted">Failures ({formatPercent(failureRate)})</div>
                </div>
              </div>
              {key.lastCorrelationId && (
                <div className="mt-3 pt-3 border-top">
                  <div className="small text-muted mb-1">Last correlation ID</div>
                  <CorrelationIdCopy correlationId={key.lastCorrelationId} />
                </div>
              )}
            </div>
          </div>
        </div>
        <div className="col-lg-4">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-header bg-white fw-semibold">Scopes</div>
            <div className="card-body">
              {key.scopes.length === 0 ? (
                <p className="small text-muted mb-0">No scopes assigned.</p>
              ) : (
                <div className="d-flex flex-wrap gap-1">
                  {key.scopes.map((scope) => (
                    <span key={scope} className="badge bg-secondary">{scope}</span>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      <ConfirmDialog
        show={rotateOpen}
        title="Rotate API key"
        message={`Rotate "${key.name}"? The current key will stop working immediately.`}
        confirmLabel="Rotate"
        confirmVariant="warning"
        isLoading={rotateMutation.isPending}
        onConfirm={() => rotateMutation.mutate()}
        onCancel={() => setRotateOpen(false)}
      />

      <ConfirmDialog
        show={revokeOpen}
        title="Revoke API key"
        message={`Revoke "${key.name}"? This cannot be undone.`}
        confirmLabel="Revoke"
        confirmVariant="danger"
        isLoading={revokeMutation.isPending}
        onConfirm={() => revokeMutation.mutate()}
        onCancel={() => setRevokeOpen(false)}
      />

      {revealedKey && (
        <KeyRevealModal
          show
          apiKey={revealedKey}
          onClose={() => setRevealedKey(null)}
        />
      )}
    </div>
  );
}
