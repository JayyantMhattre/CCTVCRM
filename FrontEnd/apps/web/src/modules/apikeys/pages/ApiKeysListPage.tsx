import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { apikeysApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { apiKeyDetailRoute } from '@/core/router/routeMap';
import { useApiKeyPermissions } from '../hooks/useApiKeyPermissions';
import { ConfirmDialog } from '@/modules/webhooks/components/ConfirmDialog';
import { KeyRevealModal } from '../components/KeyRevealModal';
import { formatUtc } from '@/modules/webhooks/utils/format';
import type { ApiKey, ApiKeyStatusFilter } from '../types';

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

function parseScopes(value: string): string[] | undefined {
  const items = value.split(',').map((s) => s.trim()).filter(Boolean);
  return items.length > 0 ? items : undefined;
}

export default function ApiKeysListPage() {
  const toast = useToast();
  const queryClient = useQueryClient();
  const { canManage } = useApiKeyPermissions();
  const { extractMessage, extractCorrelationId } = useApiError();

  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<ApiKeyStatusFilter>('');
  const [environmentFilter, setEnvironmentFilter] = useState('');
  const [showCreate, setShowCreate] = useState(false);
  const [rotateTarget, setRotateTarget] = useState<ApiKey | null>(null);
  const [revokeTarget, setRevokeTarget] = useState<ApiKey | null>(null);
  const [revealedKey, setRevealedKey] = useState<string | null>(null);
  const [formName, setFormName] = useState('');
  const [formDescription, setFormDescription] = useState('');
  const [formScopes, setFormScopes] = useState('');
  const [formExpires, setFormExpires] = useState('');

  const { data = [], isLoading, error } = useQuery({
    queryKey: ['apikeys', 'list'],
    queryFn: apikeysApi.list,
  });

  const environments = useMemo(
    () => [...new Set(data.map((k) => k.environment))].sort(),
    [data],
  );

  const filtered = useMemo(() => {
    const term = search.trim().toLowerCase();
    return data.filter((key) => {
      if (term) {
        const haystack = `${key.name} ${key.keyPrefix} ${key.description}`.toLowerCase();
        if (!haystack.includes(term)) return false;
      }
      if (environmentFilter && key.environment !== environmentFilter) return false;
      if (statusFilter === 'active' && !isActive(key)) return false;
      if (statusFilter === 'revoked' && !key.revokedOnUtc) return false;
      if (statusFilter === 'expired' && !isExpired(key)) return false;
      return true;
    });
  }, [data, search, statusFilter, environmentFilter]);

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['apikeys'] });

  const createMutation = useMutation({
    mutationFn: () =>
      apikeysApi.create({
        name: formName.trim(),
        description: formDescription.trim() || undefined,
        scopes: parseScopes(formScopes),
        expiresOnUtc: formExpires ? new Date(formExpires).toISOString() : null,
      }),
    onSuccess: (result) => {
      invalidate();
      setShowCreate(false);
      resetForm();
      setRevealedKey(result.plaintextKey);
      toast.success('API key created.');
    },
  });

  const rotateMutation = useMutation({
    mutationFn: (id: string) => apikeysApi.rotate(id),
    onSuccess: (result) => {
      invalidate();
      setRotateTarget(null);
      setRevealedKey(result.plaintextKey);
      toast.success('API key rotated. Copy the new key now.');
    },
  });

  const revokeMutation = useMutation({
    mutationFn: (id: string) => apikeysApi.revoke(id),
    onSuccess: () => {
      invalidate();
      setRevokeTarget(null);
      toast.success('API key revoked.');
    },
  });

  function resetForm() {
    setFormName('');
    setFormDescription('');
    setFormScopes('');
    setFormExpires('');
  }

  function openCreate() {
    resetForm();
    setShowCreate(true);
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
        title="API keys"
        subtitle={`${filtered.length} of ${data.length} key${data.length !== 1 ? 's' : ''}`}
      >
        {canManage && (
          <button type="button" className="btn btn-primary btn-sm" onClick={openCreate}>
            Create API key
          </button>
        )}
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          <div className="row g-2">
            <div className="col-md-4">
              <input
                className="form-control form-control-sm"
                placeholder="Search by name or prefix…"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </div>
            <div className="col-md-3">
              <select
                className="form-select form-select-sm"
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value as ApiKeyStatusFilter)}
              >
                <option value="">All statuses</option>
                <option value="active">Active</option>
                <option value="revoked">Revoked</option>
                <option value="expired">Expired</option>
              </select>
            </div>
            <div className="col-md-3">
              <select
                className="form-select form-select-sm"
                value={environmentFilter}
                onChange={(e) => setEnvironmentFilter(e.target.value)}
              >
                <option value="">All environments</option>
                {environments.map((env) => (
                  <option key={env} value={env}>{env}</option>
                ))}
              </select>
            </div>
          </div>
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState
              icon="lock"
              title={data.length === 0 ? 'No API keys' : 'No matching keys'}
              description={
                data.length === 0
                  ? 'Create an API key for machine-to-machine access.'
                  : 'Try adjusting your search or filters.'
              }
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
                  <th>Prefix</th>
                  <th>Environment</th>
                  <th>Status</th>
                  <th>Scopes</th>
                  <th>Last used</th>
                  <th>Created</th>
                  <th aria-label="Actions" />
                </tr>
              </thead>
              <tbody>
                {filtered.map((key) => (
                  <tr key={key.id}>
                    <td className="fw-semibold">{key.name}</td>
                    <td className="font-monospace small">{key.keyPrefix}…</td>
                    <td className="small">{key.environment}</td>
                    <td><KeyStatusBadge apiKey={key} /></td>
                    <td className="small">{key.scopes.length || '—'}</td>
                    <td className="small">{formatUtc(key.lastUsedOnUtc)}</td>
                    <td className="small">{formatUtc(key.createdOnUtc)}</td>
                    <td className="text-end">
                      <Link
                        to={apiKeyDetailRoute(key.id)}
                        className="btn btn-sm btn-outline-secondary me-1"
                      >
                        Details
                      </Link>
                      {canManage && isActive(key) && (
                        <>
                          <button
                            type="button"
                            className="btn btn-sm btn-outline-warning me-1"
                            onClick={() => setRotateTarget(key)}
                          >
                            Rotate
                          </button>
                          <button
                            type="button"
                            className="btn btn-sm btn-outline-danger"
                            onClick={() => setRevokeTarget(key)}
                          >
                            Revoke
                          </button>
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

      <Modal show={showCreate} onHide={() => setShowCreate(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Create API key</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="mb-3">
            <label className="form-label">Name</label>
            <input
              className="form-control"
              value={formName}
              onChange={(e) => setFormName(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label className="form-label">Description</label>
            <input
              className="form-control"
              value={formDescription}
              onChange={(e) => setFormDescription(e.target.value)}
            />
          </div>
          <div className="mb-3">
            <label className="form-label">Scopes (comma-separated)</label>
            <input
              className="form-control"
              value={formScopes}
              onChange={(e) => setFormScopes(e.target.value)}
              placeholder="users:read, audit:read"
            />
          </div>
          <div className="mb-0">
            <label className="form-label">Expires on (optional)</label>
            <input
              type="datetime-local"
              className="form-control"
              value={formExpires}
              onChange={(e) => setFormExpires(e.target.value)}
            />
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={() => setShowCreate(false)}>Cancel</Button>
          <Button
            variant="primary"
            disabled={!formName.trim() || createMutation.isPending}
            onClick={() => createMutation.mutate()}
          >
            Create
          </Button>
        </Modal.Footer>
      </Modal>

      <ConfirmDialog
        show={!!rotateTarget}
        title="Rotate API key"
        message={`Rotate "${rotateTarget?.name}"? The current key will stop working immediately.`}
        confirmLabel="Rotate"
        confirmVariant="warning"
        isLoading={rotateMutation.isPending}
        onConfirm={() => rotateTarget && rotateMutation.mutate(rotateTarget.id)}
        onCancel={() => setRotateTarget(null)}
      />

      <ConfirmDialog
        show={!!revokeTarget}
        title="Revoke API key"
        message={`Revoke "${revokeTarget?.name}"? This cannot be undone.`}
        confirmLabel="Revoke"
        confirmVariant="danger"
        isLoading={revokeMutation.isPending}
        onConfirm={() => revokeTarget && revokeMutation.mutate(revokeTarget.id)}
        onCancel={() => setRevokeTarget(null)}
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
