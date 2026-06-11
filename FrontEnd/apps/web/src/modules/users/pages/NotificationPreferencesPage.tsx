/**
 * NotificationPreferencesPage — email notification toggle for the signed-in user.
 */

import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { usersApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useAuth } from '@/shared/hooks/useAuth';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast';
import { userProfileRoute } from '@/core/router/routeMap';

export default function NotificationPreferencesPage() {
  const { user } = useAuth();
  const userId = user?.userId ?? '';
  const queryClient = useQueryClient();
  const { extractMessage, extractCorrelationId } = useApiError();
  const toast = useToast();

  const [emailEnabled, setEmailEnabled] = useState(true);

  const { data: profile, isLoading, error } = useQuery({
    queryKey: ['users', userId, 'preferences'],
    queryFn: () => usersApi.getById(userId),
    enabled: Boolean(userId),
  });

  useEffect(() => {
    if (profile) {
      setEmailEnabled(profile.preferences.emailNotificationsEnabled);
    }
  }, [profile]);

  const saveMutation = useMutation({
    mutationFn: () =>
      usersApi.updatePreferences(userId, { emailNotificationsEnabled: emailEnabled }),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ['users', userId] });
      toast.success('Notification preferences saved.');
    },
  });

  if (!userId) {
    return <AlertMessage variant="warning" message="Sign in to manage notification preferences." />;
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
        title="Notification Preferences"
        subtitle="Control how Ashraak contacts you by email."
      >
        <Link to={userProfileRoute(userId)} className="btn btn-outline-secondary btn-sm">
          View profile
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm" style={{ maxWidth: '520px' }}>
        <div className="card-body">
          <div className="form-check form-switch mb-3">
            <input
              className="form-check-input"
              type="checkbox"
              id="emailNotifications"
              checked={emailEnabled}
              onChange={(e) => setEmailEnabled(e.target.checked)}
              disabled={saveMutation.isPending}
            />
            <label className="form-check-label" htmlFor="emailNotifications">
              Email notifications enabled
            </label>
            <div className="form-text">
              When disabled, transactional emails (invites, password reset) may still be sent per tenant policy.
            </div>
          </div>

          <button
            type="button"
            className="btn btn-primary"
            disabled={saveMutation.isPending}
            onClick={() => saveMutation.mutate()}
          >
            {saveMutation.isPending ? 'Saving…' : 'Save preferences'}
          </button>
        </div>
      </div>
    </div>
  );
}
