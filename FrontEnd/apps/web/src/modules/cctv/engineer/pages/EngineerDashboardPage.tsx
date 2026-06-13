import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

interface EngineerDashboard {
  readonly engineerId: string;
  readonly todayScheduleCount: number;
  readonly openTicketCount: number;
}

function mapDashboard(raw: Record<string, unknown>): EngineerDashboard {
  return {
    engineerId: String(raw.engineerId ?? raw.EngineerId ?? ''),
    todayScheduleCount: Number(raw.todayScheduleCount ?? raw.TodayScheduleCount ?? 0),
    openTicketCount: Number(raw.openTicketCount ?? raw.OpenTicketCount ?? 0),
  };
}

export default function EngineerDashboardPage() {
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'engineer', 'dashboard'],
    queryFn: async () => {
      const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.engineer.dashboard);
      return mapDashboard(res.data);
    },
  });

  if (isLoading) return <Spinner fullPage />;
  if (error) return <AlertMessage message={extractMessage(error)} variant="danger" />;

  return (
    <div>
      <PageHeader title="My Day" subtitle="Engineer dashboard" />

      <div className="row g-3 mb-4">
        <div className="col-md-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <div className="text-muted small">Today&apos;s visits</div>
              <div className="display-6 fw-semibold">{data?.todayScheduleCount ?? 0}</div>
              <Link to={ROUTES.cctv.engineer.visits} className="btn btn-sm btn-outline-primary mt-2">
                View assigned visits
              </Link>
            </div>
          </div>
        </div>
        <div className="col-md-6">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body">
              <div className="text-muted small">Open tickets</div>
              <div className="display-6 fw-semibold">{data?.openTicketCount ?? 0}</div>
              <Link to={ROUTES.cctv.engineer.tickets} className="btn btn-sm btn-outline-primary mt-2">
                View assigned tickets
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
