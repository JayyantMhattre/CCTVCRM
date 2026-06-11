/**
 * TenantSettingsPage — workspace security and notification defaults.
 */

import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { tenantApi, DEFAULT_TENANT_SETTINGS } from '../api';
import type { TenantSettingsDto } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useApiError } from '@/shared/hooks/useApiError';
import { useAuth } from '@/shared/hooks/useAuth';
import { useToast } from '@/shared/ui/toast';

export default function TenantSettingsPage() {
  const queryClient = useQueryClient();
  const { hasRole } = useAuth();
  const canEdit = hasRole('Admin', 'Manager');
  const { extractMessage, extractCorrelationId } = useApiError();
  const toast = useToast();

  const [form, setForm] = useState<TenantSettingsDto>({ ...DEFAULT_TENANT_SETTINGS });

  const { data: settings, isLoading, error } = useQuery({
    queryKey: ['tenant', 'settings'],
    queryFn: () => tenantApi.getSettings(),
    staleTime: 30_000,
  });

  useEffect(() => {
    if (settings) setForm(settings);
  }, [settings]);

  const saveMutation = useMutation({
    mutationFn: () => tenantApi.updateSettings(form),
    onSuccess: (saved) => {
      setForm(saved);
      void queryClient.invalidateQueries({ queryKey: ['tenant', 'settings'] });
      toast.success('Tenant settings saved.');
    },
  });

  function updateField<K extends keyof TenantSettingsDto>(key: K, value: TenantSettingsDto[K]) {
    setForm((prev) => ({ ...prev, [key]: value }));
  }

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    const correlationId = extractCorrelationId(error);
    return (
      <div>
        <AlertMessage message={extractMessage(error)} variant="danger" />
        {correlationId && <CorrelationIdCopy correlationId={correlationId} />}
      </div>
    );
  }

  return (
    <div>
      <PageHeader
        title="Tenant Settings"
        subtitle="Security, locale, and session defaults for your workspace."
      />

      {!canEdit && (
        <AlertMessage
          variant="warning"
          message="Only tenant administrators can change workspace settings. You can view defaults below."
        />
      )}

      <div className="card border-0 shadow-sm" style={{ maxWidth: '640px' }}>
          <div className="card-body">
            <h6 className="text-muted text-uppercase small mb-3">Security</h6>

            <div className="form-check form-switch mb-3">
              <input
                className="form-check-input"
                type="checkbox"
                id="requireMfa"
                checked={form.requireMfa}
                onChange={(e) => updateField('requireMfa', e.target.checked)}
                disabled={!canEdit}
              />
              <label className="form-check-label" htmlFor="requireMfa">
                Require MFA for all users
              </label>
            </div>

            <div className="mb-3">
              <label className="form-label" htmlFor="sessionTimeout">
                Session timeout (minutes)
              </label>
              <input
                id="sessionTimeout"
                type="number"
                min={5}
                max={1440}
                className="form-control"
                value={form.sessionTimeoutMinutes}
                onChange={(e) => updateField('sessionTimeoutMinutes', Number(e.target.value))}
                disabled={!canEdit}
              />
            </div>

            <h6 className="text-muted text-uppercase small mb-3 mt-4">Locale</h6>

            <div className="mb-3">
              <label className="form-label" htmlFor="locale">Locale</label>
              <input
                id="locale"
                type="text"
                className="form-control"
                value={form.locale}
                onChange={(e) => updateField('locale', e.target.value)}
                placeholder="en-US"
                disabled={!canEdit}
              />
            </div>

            <div className="mb-3">
              <label className="form-label" htmlFor="timezone">Timezone</label>
              <input
                id="timezone"
                type="text"
                className="form-control"
                value={form.timezone}
                onChange={(e) => updateField('timezone', e.target.value)}
                placeholder="UTC"
                disabled={!canEdit}
              />
            </div>

            <h6 className="text-muted text-uppercase small mb-3 mt-4">Notifications</h6>
            <p className="small text-muted mb-3">
              Per-user email preferences are managed under Notification Preferences.
              Tenant-level notification routing will apply when the notifications API exposes tenant defaults.
            </p>

            <button
              type="button"
              className="btn btn-primary"
              disabled={!canEdit || saveMutation.isPending}
              onClick={() => saveMutation.mutate()}
            >
              {saveMutation.isPending ? 'Saving…' : 'Save settings'}
            </button>
          </div>
        </div>
    </div>
  );
}
