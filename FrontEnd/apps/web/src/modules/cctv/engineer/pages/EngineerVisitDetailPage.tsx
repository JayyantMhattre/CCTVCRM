import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { visitsApi } from '../../visits/api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

function visitReportPath(visitId: string): string {
  return ROUTES.cctv.engineer.visitReport.replace(':visitId', visitId);
}

export default function EngineerVisitDetailPage() {
  const { visitId = '' } = useParams<{ visitId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();

  const visitQuery = useQuery({
    queryKey: ['cctv', 'engineer', 'visits', visitId],
    queryFn: () => visitsApi.getEngineerVisit(visitId),
    enabled: Boolean(visitId),
  });

  const startMutation = useMutation({
    mutationFn: () => visitsApi.start(visitId, visitQuery.data!.rowVersion),
    onSuccess: () => {
      toast.success('Visit started');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'engineer', 'visits', visitId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (visitQuery.isLoading) return <Spinner fullPage />;
  if (visitQuery.error) return <AlertMessage message={extractMessage(visitQuery.error)} variant="danger" />;

  const visit = visitQuery.data;
  if (!visit) return null;

  const canStart = !visit.startedAtUtc && visit.reportStatus === 'Draft';
  const canReport = Boolean(visit.startedAtUtc) && visit.reportStatus !== 'Approved';

  return (
    <div>
      <PageHeader title={visit.scheduleNumber} subtitle={`Visit · ${visit.reportStatus}`}>
        <Link to={ROUTES.cctv.engineer.visits} className="btn btn-outline-secondary btn-sm">
          Back to visits
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          <dl className="row mb-0">
            <dt className="col-sm-3">Site</dt>
            <dd className="col-sm-9">{visit.siteId}</dd>
            <dt className="col-sm-3">Started</dt>
            <dd className="col-sm-9">{visit.startedAtUtc ? new Date(visit.startedAtUtc).toLocaleString() : 'Not started'}</dd>
          </dl>
        </div>
      </div>

      <div className="d-flex flex-wrap gap-2">
        {canStart && (
          <button type="button" className="btn btn-primary btn-sm" disabled={startMutation.isPending} onClick={() => startMutation.mutate()}>
            Start visit
          </button>
        )}
        {canReport && (
          <Link to={visitReportPath(visitId)} className="btn btn-outline-primary btn-sm">
            Open visit report
          </Link>
        )}
      </div>
    </div>
  );
}
