/**
 * ForbiddenPage — displayed when a user lacks the required role or permission.
 * Route: /403
 */

import { Link } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';

export default function ForbiddenPage() {
  return (
    <div className="d-flex flex-column justify-content-center align-items-center min-vh-100 bg-light">
      <div className="text-center p-4">
        <i className="bi bi-shield-lock display-1 text-warning mb-3 d-block" aria-hidden="true" />
        <h1 className="display-6 fw-bold text-dark">403 — Access Denied</h1>
        <p className="text-muted mb-4">
          You don't have permission to view this page.
          Contact your administrator if you believe this is a mistake.
        </p>
        <Link to={ROUTES.dashboard} className="btn btn-primary">
          <i className="bi bi-house me-2" aria-hidden="true" />
          Back to Dashboard
        </Link>
      </div>
    </div>
  );
}
