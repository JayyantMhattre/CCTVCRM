/**
 * UserListPage — lists all users in the current tenant.
 *
 * Access: Admin or Manager roles (enforced by RoleGuard in the router).
 * Fetches users for the JWT-resolved tenant so no manual tenant selection
 * is needed in the UI.
 */

import { useQuery }     from '@tanstack/react-query';
import { Link }         from 'react-router-dom';
import { usersApi }     from '../api';
import { PageHeader }   from '@/shared/components/PageHeader';
import { Spinner }      from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState }   from '@/shared/components/EmptyState';
import { useApiError }  from '@/shared/hooks/useApiError';
import { ROUTES }       from '@/core/router/routeMap';
import type { UserDto } from '../types';

function StatusBadge({ status }: { status: UserDto['status'] }) {
  const map: Record<UserDto['status'], string> = {
    Active:    'bg-success',
    Inactive:  'bg-secondary',
    Suspended: 'bg-warning text-dark',
  };
  return <span className={`badge ${map[status]}`}>{status}</span>;
}

export default function UserListPage() {
  const { extractMessage } = useApiError();

  const { data: users = [], isLoading, error } = useQuery({
    queryKey: ['users', 'current-tenant'],
    queryFn:  () => usersApi.listForCurrentTenant(),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return (
      <AlertMessage
        message={extractMessage(error)}
        variant="danger"
        icon="exclamation-circle"
      />
    );
  }

  return (
    <div>
      <PageHeader
        title="Users"
        subtitle={`${users.length} member${users.length !== 1 ? 's' : ''} in this workspace`}
      />

      {users.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState
              icon="people"
              title="No users yet"
              description="Users who register under this tenant will appear here."
            />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>User</th>
                  <th>Email</th>
                  <th>Status</th>
                  <th>Joined</th>
                  <th aria-label="Actions" />
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.userId}>
                    <td>
                      <div className="d-flex align-items-center gap-2">
                        {/* Avatar */}
                        <div
                          className="rounded-circle bg-primary d-flex justify-content-center align-items-center text-white fw-bold flex-shrink-0"
                          style={{ width: '32px', height: '32px', fontSize: '0.8rem' }}
                          aria-hidden="true"
                        >
                          {user.displayName.charAt(0).toUpperCase()}
                        </div>
                        <span className="fw-semibold">{user.displayName}</span>
                      </div>
                    </td>
                    <td className="text-muted">{user.email}</td>
                    <td><StatusBadge status={user.status} /></td>
                    <td className="text-muted small">
                      {new Date(user.createdOnUtc).toLocaleDateString()}
                    </td>
                    <td className="text-end">
                      <Link
                        to={ROUTES.users.profile.replace(':userId', user.userId)}
                        className="btn btn-sm btn-outline-secondary"
                      >
                        View
                      </Link>
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
