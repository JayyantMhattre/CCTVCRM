/**
 * NotFoundPage — catch-all 404 page.
 * Route: * (matched last in the router)
 */

import { Link } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';

export default function NotFoundPage() {
  return (
    <div className="d-flex flex-column justify-content-center align-items-center min-vh-100 bg-light">
      <div className="text-center p-4">
        <i className="bi bi-map display-1 text-secondary mb-3 d-block" aria-hidden="true" />
        <h1 className="display-6 fw-bold text-dark">404 — Page Not Found</h1>
        <p className="text-muted mb-4">
          The page you're looking for doesn't exist or has been moved.
        </p>
        <Link to={ROUTES.dashboard} className="btn btn-primary">
          <i className="bi bi-house me-2" aria-hidden="true" />
          Back to Dashboard
        </Link>
      </div>
    </div>
  );
}
