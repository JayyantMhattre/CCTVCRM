import { Link } from 'react-router-dom';
import { PageHeader } from '@/shared/components/PageHeader';
import { ROUTES } from '@/core/router/routeMap';

export default function EngineerProfilePage() {
  return (
    <div>
      <PageHeader title="Profile & Preferences" subtitle="Platform account settings reused for engineers" />

      <div className="row g-3">
        <div className="col-md-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h2 className="h6 fw-semibold">Account profile</h2>
              <p className="text-muted small">View avatar, tenant, and role assignments.</p>
              <Link to={ROUTES.tenant.profile} className="btn btn-sm btn-outline-primary">
                Open profile
              </Link>
            </div>
          </div>
        </div>
        <div className="col-md-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h2 className="h6 fw-semibold">Notification preferences</h2>
              <p className="text-muted small">Configure email and in-app notification toggles.</p>
              <Link to={ROUTES.users.preferences} className="btn btn-sm btn-outline-primary">
                Notification settings
              </Link>
            </div>
          </div>
        </div>
        <div className="col-md-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <h2 className="h6 fw-semibold">Active sessions</h2>
              <p className="text-muted small">Review signed-in devices and revoke sessions.</p>
              <Link to={ROUTES.sessions} className="btn btn-sm btn-outline-primary">
                Manage sessions
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
