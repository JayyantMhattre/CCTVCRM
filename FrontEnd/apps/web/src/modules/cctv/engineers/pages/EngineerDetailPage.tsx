import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { engineersApi } from '../api';
import type { EngineerStatus } from '../types';
import { usersApi } from '@/modules/users/api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

export default function EngineerDetailPage() {
  const { engineerId = '' } = useParams<{ engineerId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();
  const [editing, setEditing] = useState(false);
  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [platformUserId, setPlatformUserId] = useState('');

  const engineerQuery = useQuery({
    queryKey: ['cctv', 'engineers', engineerId],
    queryFn: () => engineersApi.get(engineerId),
    enabled: Boolean(engineerId),
  });

  const usersQuery = useQuery({
    queryKey: ['users', 'tenant'],
    queryFn: () => usersApi.listForCurrentTenant(),
  });

  const workloadQuery = useQuery({
    queryKey: ['cctv', 'engineers', engineerId, 'workload'],
    queryFn: () => engineersApi.workload(engineerId),
    enabled: Boolean(engineerId),
  });

  useEffect(() => {
    if (engineerQuery.data) {
      setName(engineerQuery.data.name);
      setPhone(engineerQuery.data.phone);
      setPlatformUserId(engineerQuery.data.platformUserId ?? '');
    }
  }, [engineerQuery.data]);

  const updateMutation = useMutation({
    mutationFn: () =>
      engineersApi.update(engineerId, {
        name: name.trim(),
        phone: phone.trim(),
        platformUserId: platformUserId || null,
        rowVersion: engineerQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Engineer updated');
      setEditing(false);
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'engineers'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const statusMutation = useMutation({
    mutationFn: (status: EngineerStatus) =>
      engineersApi.changeStatus(engineerId, {
        status,
        rowVersion: engineerQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Engineer status updated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'engineers'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (engineerQuery.isLoading) return <Spinner fullPage />;

  if (engineerQuery.error) {
    return <AlertMessage message={extractMessage(engineerQuery.error)} variant="danger" />;
  }

  const engineer = engineerQuery.data;
  if (!engineer) return null;

  const nextStatus: EngineerStatus | null =
    engineer.status === 'Active' ? 'Inactive' : engineer.status === 'Inactive' ? 'Active' : null;

  const workload = workloadQuery.data;

  return (
    <div>
      <PageHeader title={engineer.name} subtitle={engineer.engineerNumber}>
        <Link to={ROUTES.cctv.admin.engineers} className="btn btn-outline-secondary btn-sm">
          Back to engineers
        </Link>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white fw-semibold d-flex justify-content-between align-items-center">
              Profile
              <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => setEditing((v) => !v)}>
                {editing ? 'Cancel' : 'Edit'}
              </button>
            </div>
            <div className="card-body">
              <span className={`badge ${engineer.status === 'Active' ? 'bg-success' : 'bg-secondary'} mb-3`}>
                {engineer.status}
              </span>
              {editing ? (
                <>
                  <div className="mb-3">
                    <label className="form-label">Name</label>
                    <input className="form-control" value={name} onChange={(e) => setName(e.target.value)} />
                  </div>
                  <div className="mb-3">
                    <label className="form-label">Phone</label>
                    <input className="form-control" value={phone} onChange={(e) => setPhone(e.target.value)} />
                  </div>
                  <div className="mb-3">
                    <label className="form-label">Platform user</label>
                    <select className="form-select" value={platformUserId} onChange={(e) => setPlatformUserId(e.target.value)}>
                      <option value="">— None —</option>
                      {(usersQuery.data ?? []).map((user) => (
                        <option key={user.userId} value={user.userId}>
                          {user.displayName} ({user.email})
                        </option>
                      ))}
                    </select>
                  </div>
                  <button
                    type="button"
                    className="btn btn-primary btn-sm"
                    disabled={updateMutation.isPending}
                    onClick={() => updateMutation.mutate()}
                  >
                    Save changes
                  </button>
                </>
              ) : (
                <dl className="row mb-0">
                  <dt className="col-sm-3">Phone</dt>
                  <dd className="col-sm-9">{engineer.phone}</dd>
                  {engineer.platformUserId && (
                    <>
                      <dt className="col-sm-3">Platform user</dt>
                      <dd className="col-sm-9">{engineer.platformUserId}</dd>
                    </>
                  )}
                  <dt className="col-sm-3">Created</dt>
                  <dd className="col-sm-9">{new Date(engineer.createdAtUtc).toLocaleString()}</dd>
                </dl>
              )}
            </div>
          </div>

          {workload && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-header bg-white fw-semibold">Workload</div>
              <div className="card-body">
                <div className="row g-3">
                  <div className="col-sm-6">
                    <div className="text-muted small">Active schedules</div>
                    <div className="fs-4 fw-semibold">{workload.activeScheduleCount}</div>
                  </div>
                  <div className="col-sm-6">
                    <div className="text-muted small">Open tickets</div>
                    <div className="fs-4 fw-semibold">{workload.openTicketCount}</div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {nextStatus && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-header bg-white fw-semibold">Status</div>
              <div className="card-body">
                <button
                  type="button"
                  className={`btn btn-sm ${nextStatus === 'Inactive' ? 'btn-outline-danger' : 'btn-outline-success'}`}
                  disabled={statusMutation.isPending}
                  onClick={() => statusMutation.mutate(nextStatus)}
                >
                  Mark {nextStatus}
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
