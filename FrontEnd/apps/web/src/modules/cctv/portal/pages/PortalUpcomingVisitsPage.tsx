import { useQuery } from '@tanstack/react-query';
import { portalApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { EmptyState } from '@/shared/components/EmptyState';
import { useApiError } from '@/shared/hooks/useApiError';

export default function PortalUpcomingVisitsPage() {
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'portal', 'visits', 'upcoming'],
    queryFn: () => portalApi.listUpcomingVisits(),
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;

  const items = data ?? [];

  return (
    <div>
      <PageHeader title="Upcoming visits" subtitle="Scheduled preventive maintenance" />

      {items.length === 0 ? (
        <div className="card border-0 shadow-sm">
          <div className="card-body">
            <EmptyState title="No upcoming visits" description="Your next AMC visit will appear here when scheduled." />
          </div>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="table-responsive">
            <table className="table table-hover mb-0 align-middle">
              <thead className="table-light">
                <tr>
                  <th>Schedule</th>
                  <th>Scheduled date</th>
                  <th>Engineer</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {items.map((schedule) => (
                  <tr key={schedule.id}>
                    <td className="fw-medium">{schedule.scheduleNumber}</td>
                    <td>{schedule.scheduledDate}</td>
                    <td className="small">{schedule.activeEngineerId ? `${schedule.activeEngineerId.slice(0, 8)}…` : 'TBD'}</td>
                    <td>{schedule.status}</td>
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
