import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

interface EngineerScheduleRow {
  readonly id: string;
  readonly scheduleNumber: string;
  readonly scheduledDate: string;
  readonly status: string;
  readonly siteId: string;
  readonly visitId: string | null;
}

function mapRow(raw: Record<string, unknown>): EngineerScheduleRow {
  return {
    id: String(raw.id ?? raw.Id ?? ''),
    scheduleNumber: String(raw.scheduleNumber ?? raw.ScheduleNumber ?? ''),
    scheduledDate: String(raw.scheduledDate ?? raw.ScheduledDate ?? ''),
    status: String(raw.status ?? raw.Status ?? ''),
    siteId: String(raw.siteId ?? raw.SiteId ?? ''),
    visitId: (raw.visitId ?? raw.VisitId ?? null) as string | null,
  };
}

function visitDetailPath(visitId: string): string {
  return ROUTES.cctv.engineer.visitDetail.replace(':visitId', visitId);
}

export default function EngineerVisitsPage() {
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'engineer', 'schedules'],
    queryFn: async () => {
      const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.engineer.schedules);
      return (res.data ?? []).map((item) => mapRow(item as Record<string, unknown>));
    },
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  const items = data ?? [];

  return (
    <div>
      <PageHeader title="My Visits" subtitle="Assigned service schedules" />

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No assigned visits" description="Schedules assigned to you will appear here." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Schedule</th>
                  <th>Date</th>
                  <th>Status</th>
                  <th className="text-end">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((row) => (
                  <tr key={row.id}>
                    <td className="fw-medium">{row.scheduleNumber}</td>
                    <td>{row.scheduledDate}</td>
                    <td>
                      <span className="badge bg-secondary">{row.status}</span>
                    </td>
                    <td className="text-end">
                      {row.visitId ? (
                        <Link to={visitDetailPath(row.visitId)} className="btn btn-sm btn-outline-primary">
                          Open visit
                        </Link>
                      ) : (
                        <span className="text-muted small">No visit yet</span>
                      )}
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
