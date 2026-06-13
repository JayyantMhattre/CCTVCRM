import { Link, Navigate } from 'react-router-dom';
import { PageHeader } from '@/shared/components/PageHeader';
import { ROUTES } from '@/core/router/routeMap';
import { useAuthStore } from '@/core/auth/authStore';
import { REPORT_CATALOG, REPORTS_READ_PERMISSION } from '../api';

function reportViewPath(key: string): string {
  return ROUTES.cctv.admin.reportView.replace(':reportKey', key);
}

export default function ReportsHubPage() {
  const permissions = useAuthStore((state) => state.user?.permissions ?? []);
  const canRead = permissions.some((p) => p.toLowerCase() === REPORTS_READ_PERMISSION);

  if (!canRead) {
    return <Navigate to={ROUTES.forbidden} replace />;
  }

  return (
    <div>
      <PageHeader title="Reports" subtitle="Role-based operational and financial reports" />

      <div className="row g-3">
        {REPORT_CATALOG.map((report) => (
          <div key={report.key} className="col-md-6 col-xl-4">
            <div className="card border-0 shadow-sm h-100">
              <div className="card-body d-flex flex-column">
                <h2 className="h6 fw-semibold">{report.title}</h2>
                <p className="text-muted small flex-grow-1">{report.description}</p>
                <Link to={reportViewPath(report.key)} className="btn btn-sm btn-outline-primary align-self-start">
                  View report
                </Link>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
