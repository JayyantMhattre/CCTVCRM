import { useState } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from 'react-router-dom';
import { engineersApi } from '../api';
import { usersApi } from '@/modules/users/api';
import { PageHeader } from '@/shared/components/PageHeader';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

export default function EngineerCreatePage() {
  const navigate = useNavigate();
  const toast = useToast();
  const { extractMessage } = useApiError();
  const [name, setName] = useState('');
  const [phone, setPhone] = useState('');
  const [platformUserId, setPlatformUserId] = useState('');

  const usersQuery = useQuery({
    queryKey: ['users', 'tenant'],
    queryFn: () => usersApi.listForCurrentTenant(),
  });

  const createMutation = useMutation({
    mutationFn: () =>
      engineersApi.create({
        name: name.trim(),
        phone: phone.trim(),
        platformUserId: platformUserId || null,
      }),
    onSuccess: (engineer) => {
      toast.success('Engineer created');
      void navigate(ROUTES.cctv.admin.engineerDetail.replace(':engineerId', engineer.id));
    },
  });

  return (
    <div>
      <PageHeader title="New engineer" subtitle="Create engineer master record">
        <Link to={ROUTES.cctv.admin.engineers} className="btn btn-outline-secondary btn-sm">
          Back
        </Link>
      </PageHeader>

      {createMutation.error && (
        <AlertMessage message={extractMessage(createMutation.error)} variant="danger" />
      )}

      <div className="card border-0 shadow-sm">
        <div className="card-body">
          <div className="mb-3">
            <label className="form-label">Name</label>
            <input className="form-control" value={name} onChange={(e) => setName(e.target.value)} required />
          </div>
          <div className="mb-3">
            <label className="form-label">Phone</label>
            <input className="form-control" value={phone} onChange={(e) => setPhone(e.target.value)} required />
          </div>
          <div className="mb-3">
            <label className="form-label">Platform user (optional)</label>
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
            className="btn btn-primary"
            disabled={createMutation.isPending || !name.trim() || !phone.trim()}
            onClick={() => createMutation.mutate()}
          >
            Create engineer
          </button>
        </div>
      </div>
    </div>
  );
}
