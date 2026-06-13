import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { visitsApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

function ChecklistItem({ label, done }: { label: string; done: boolean }) {
  return (
    <li className={`list-group-item d-flex justify-content-between align-items-center ${done ? 'list-group-item-success' : ''}`}>
      {label}
      <span className={`badge ${done ? 'bg-success' : 'bg-secondary'}`}>{done ? 'Yes' : 'No'}</span>
    </li>
  );
}

export default function VisitDetailPage() {
  const { visitId = '' } = useParams();
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'visits', visitId],
    queryFn: () => visitsApi.get(visitId),
    enabled: Boolean(visitId),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  if (!data) {
    return <AlertMessage message="Visit not found." variant="warning" icon="exclamation-triangle" />;
  }

  return (
    <div>
      <PageHeader title={data.scheduleNumber} subtitle={`Report status: ${data.reportStatus}`}>
        <Link to={ROUTES.cctv.admin.visits} className="btn btn-outline-secondary btn-sm">
          Back to visits
        </Link>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-6">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white fw-semibold">Visit details</div>
            <div className="card-body">
              <dl className="row mb-0">
                <dt className="col-sm-4">Visit ID</dt>
                <dd className="col-sm-8 text-muted small">{data.id}</dd>
                <dt className="col-sm-4">Engineer</dt>
                <dd className="col-sm-8 text-muted small">{data.engineerId}</dd>
                <dt className="col-sm-4">Started</dt>
                <dd className="col-sm-8">{data.startedAtUtc ?? '—'}</dd>
                <dt className="col-sm-4">Completed</dt>
                <dd className="col-sm-8">{data.completedAtUtc ?? '—'}</dd>
                <dt className="col-sm-4">Customer visible</dt>
                <dd className="col-sm-8">{data.isCustomerVisible ? 'Yes' : 'No'}</dd>
              </dl>
            </div>
          </div>
        </div>

        <div className="col-lg-6">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white fw-semibold">Evidence checklist (BR-VISIT-01)</div>
            <ul className="list-group list-group-flush">
              <ChecklistItem label="Selfie" done={data.hasSelfie} />
              <ChecklistItem label="Before/During/After photo" done={data.hasBeforeDuringAfterPhoto} />
              <ChecklistItem label="GPS location" done={data.hasGps} />
              <ChecklistItem label="Customer signature" done={data.hasSignature} />
              <ChecklistItem label="Remarks (min 20 chars)" done={data.hasMinimumRemarks} />
            </ul>
          </div>
        </div>

        {data.visitRemarks && (
          <div className="col-12">
            <div className="card border-0 shadow-sm">
              <div className="card-header bg-white fw-semibold">Remarks</div>
              <div className="card-body">
                <p className="mb-0">{data.visitRemarks}</p>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
