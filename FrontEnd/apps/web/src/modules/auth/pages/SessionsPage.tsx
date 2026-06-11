import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { authApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';

export default function SessionsPage() {
  const queryClient = useQueryClient();
  const { extractMessage, extractCorrelationId } = useApiError();
  const toast = useToast();

  const { data: sessions = [], isLoading, error } = useQuery({
    queryKey: ['auth', 'sessions'],
    queryFn: () => authApi.listSessions(),
  });

  const revokeMutation = useMutation({
    mutationFn: (sessionId: string) => authApi.revokeSession(sessionId),
    onSuccess: () => {
      toast.success('Session revoked.');
      void queryClient.invalidateQueries({ queryKey: ['auth', 'sessions'] });
    },
  });

  const revokeAllMutation = useMutation({
    mutationFn: () => authApi.revokeAllSessions(),
    onSuccess: () => {
      toast.success('All sessions revoked.');
      void queryClient.invalidateQueries({ queryKey: ['auth', 'sessions'] });
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

  return (
    <div>
      <PageHeader title="Devices & Sessions" subtitle="Active sign-in sessions for your account.">
        <button
          type="button"
          className="btn btn-outline-danger btn-sm"
          disabled={revokeAllMutation.isPending || sessions.length === 0}
          onClick={() => revokeAllMutation.mutate()}
        >
          Revoke all
        </button>
      </PageHeader>

      {sessions.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState icon="devices" title="No active sessions" description="Sign in to create a session record." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0 small">
              <thead className="table-light">
                <tr>
                  <th>Created</th>
                  <th>Last used</th>
                  <th>IP</th>
                  <th>Device</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {sessions.map((s) => (
                  <tr key={s.id}>
                    <td>{new Date(s.createdOnUtc).toLocaleString()}</td>
                    <td>{new Date(s.lastUsedOnUtc).toLocaleString()}</td>
                    <td>{s.ipAddress}</td>
                    <td className="text-truncate" style={{ maxWidth: 240 }}>{s.userAgent}</td>
                    <td className="text-end">
                      <button
                        type="button"
                        className="btn btn-sm btn-outline-secondary"
                        disabled={revokeMutation.isPending}
                        onClick={() => revokeMutation.mutate(s.id)}
                      >
                        Revoke
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
