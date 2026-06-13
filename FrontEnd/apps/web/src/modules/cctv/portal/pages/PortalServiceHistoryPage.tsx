import { useQuery } from '@tanstack/react-query';
import { portalApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';

export default function PortalServiceHistoryPage() {
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'portal', 'visits', 'history'],
    queryFn: () => portalApi.listVisitHistory(),
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;

  const items = data ?? [];

  return (
    <div>
      <PageHeader title="Service history" subtitle="Approved visit reports for your sites" />

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No service history" description="Completed visits will appear here after admin approval." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Schedule</th>
                  <th>Visit date</th>
                  <th>Engineer</th>
                  <th>Status</th>
                  <th>Evidence</th>
                </tr>
              </thead>
              <tbody>
                {items.map((visit) => (
                  <tr key={visit.id}>
                    <td className="fw-medium">{visit.scheduleNumber}</td>
                    <td className="small text-muted">{visit.completedAtUtc ?? visit.startedAtUtc ?? '—'}</td>
                    <td className="small">{visit.engineerId.slice(0, 8)}…</td>
                    <td>{visit.reportStatus}</td>
                    <td className="small text-muted">{visit.reportStatus === 'Approved' ? 'Available' : 'Pending'}</td>
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
