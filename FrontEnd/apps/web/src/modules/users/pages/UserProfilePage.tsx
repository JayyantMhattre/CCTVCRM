/**
 * UserProfilePage — shows a single user's profile detail.
 *
 * Reads the `userId` from the URL param (`:userId` in the route).
 */

import { useQuery }     from '@tanstack/react-query';
import { useParams, Link }  from 'react-router-dom';
import { usersApi }     from '../api';
import { PageHeader }   from '@/shared/components/PageHeader';
import { Spinner }      from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError }  from '@/shared/hooks/useApiError';
import { ROUTES }       from '@/core/router/routeMap';
import { format }       from 'date-fns';

export default function UserProfilePage() {
  const { userId }         = useParams<{ userId: string }>();
  const { extractMessage } = useApiError();

  const { data: user, isLoading, error } = useQuery({
    queryKey: ['users', userId],
    queryFn:  () => usersApi.getById(userId!),
    enabled:  Boolean(userId),
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

  if (!user) return null;

  return (
    <div>
      <PageHeader title={user.displayName} subtitle={user.email}>
        <Link to={ROUTES.users.list} className="btn btn-outline-secondary btn-sm">
          <i className="bi bi-arrow-left me-2" aria-hidden="true" />
          Back to Users
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm" style={{ maxWidth: '600px' }}>
        <div className="card-body">
          <dl className="row mb-0">
            <dt className="col-sm-5 text-muted">Display Name</dt>
            <dd className="col-sm-7 fw-semibold">{user.displayName}</dd>

            <dt className="col-sm-5 text-muted">Email</dt>
            <dd className="col-sm-7">{user.email}</dd>

            <dt className="col-sm-5 text-muted">Status</dt>
            <dd className="col-sm-7">
              <span className={`badge ${
                user.status === 'Active' ? 'bg-success' : 'bg-secondary'
              }`}>
                {user.status}
              </span>
            </dd>

            <dt className="col-sm-5 text-muted">Theme</dt>
            <dd className="col-sm-7">{user.preferences.theme}</dd>

            <dt className="col-sm-5 text-muted">Locale</dt>
            <dd className="col-sm-7">{user.preferences.locale}</dd>

            <dt className="col-sm-5 text-muted">Timezone</dt>
            <dd className="col-sm-7">{user.preferences.timezone}</dd>

            <dt className="col-sm-5 text-muted">Email Notifications</dt>
            <dd className="col-sm-7 d-flex align-items-center gap-2">
              {user.preferences.emailNotificationsEnabled ? 'Enabled' : 'Disabled'}
              <Link to={ROUTES.users.preferences} className="btn btn-link btn-sm p-0">
                Manage
              </Link>
            </dd>

            <dt className="col-sm-5 text-muted">Member Since</dt>
            <dd className="col-sm-7">
              {format(new Date(user.createdOnUtc), 'PPP')}
            </dd>

            <dt className="col-sm-5 text-muted">User ID</dt>
            <dd className="col-sm-7">
              <code className="text-muted small">{user.userId}</code>
            </dd>
          </dl>
        </div>
      </div>
    </div>
  );
}
