import { useRef, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { visitsApi } from '../../visits/api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

function visitDetailPath(visitId: string): string {
  return ROUTES.cctv.engineer.visitDetail.replace(':visitId', visitId);
}

export default function EngineerVisitReportPage() {
  const { visitId = '' } = useParams<{ visitId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();
  const [remarks, setRemarks] = useState('');
  const [signerName, setSignerName] = useState('');
  const canvasRef = useRef<HTMLCanvasElement>(null);

  const visitQuery = useQuery({
    queryKey: ['cctv', 'engineer', 'visits', visitId],
    queryFn: () => visitsApi.getEngineerVisit(visitId),
    enabled: Boolean(visitId),
  });

  const invalidate = () => {
    void queryClient.invalidateQueries({ queryKey: ['cctv', 'engineer', 'visits', visitId] });
  };

  const selfieMutation = useMutation({
    mutationFn: (file: File) => visitsApi.linkSelfie(visitId, file),
    onSuccess: () => { toast.success('Selfie linked'); invalidate(); },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const photoMutation = useMutation({
    mutationFn: ({ file, category }: { file: File; category: 'Before' | 'During' | 'After' }) =>
      visitsApi.linkPhoto(visitId, file, category),
    onSuccess: () => { toast.success('Photo linked'); invalidate(); },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const gpsMutation = useMutation({
    mutationFn: () =>
      new Promise<{ latitude: number; longitude: number }>((resolve, reject) => {
        if (!navigator.geolocation) {
          reject(new Error('Geolocation is not supported in this browser.'));
          return;
        }
        navigator.geolocation.getCurrentPosition(
          (pos) => resolve({ latitude: pos.coords.latitude, longitude: pos.coords.longitude }),
          (err) => reject(err),
        );
      }).then(({ latitude, longitude }) => visitsApi.captureLocation(visitId, latitude, longitude)),
    onSuccess: () => { toast.success('GPS captured'); invalidate(); },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const remarksMutation = useMutation({
    mutationFn: () => visitsApi.updateRemarks(visitId, remarks, visitQuery.data!.rowVersion),
    onSuccess: () => { toast.success('Remarks saved'); invalidate(); },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const signatureMutation = useMutation({
    mutationFn: async () => {
      const canvas = canvasRef.current;
      if (!canvas || !signerName.trim()) throw new Error('Signature and signer name are required.');
      const blob = await new Promise<Blob | null>((resolve) => canvas.toBlob(resolve, 'image/png'));
      if (!blob) throw new Error('Could not export signature.');
      const file = new File([blob], 'signature.png', { type: 'image/png' });
      return visitsApi.linkSignature(visitId, file, signerName.trim());
    },
    onSuccess: () => { toast.success('Signature linked'); invalidate(); },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const submitMutation = useMutation({
    mutationFn: () => visitsApi.submitReport(visitId, visitQuery.data!.rowVersion),
    onSuccess: () => {
      toast.success('Visit report submitted for approval');
      invalidate();
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const videoMutation = useMutation({
    mutationFn: (file: File) => visitsApi.linkVideo(visitId, file),
    onSuccess: () => { toast.success('Video linked'); invalidate(); },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (visitQuery.isLoading) return <Spinner fullPage />;
  if (visitQuery.error) return <AlertMessage message={extractMessage(visitQuery.error)} variant="danger" />;

  const visit = visitQuery.data;
  if (!visit) return null;

  const videoAttachments = visit.attachments.filter((a) => a.attachmentType === 'Video');

  const readyToSubmit =
    visit.hasSelfie &&
    visit.hasGps &&
    visit.hasBeforeDuringAfterPhoto &&
    visit.hasSignature &&
    visit.hasMinimumRemarks;

  return (
    <div>
      <PageHeader title="Visit report" subtitle={visit.scheduleNumber}>
        <Link to={visitDetailPath(visitId)} className="btn btn-outline-secondary btn-sm">
          Back to visit
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-header bg-white fw-semibold">Evidence checklist (BR-VISIT-01)</div>
        <ul className="list-group list-group-flush">
          <li className="list-group-item d-flex justify-content-between">
            Engineer selfie {visit.hasSelfie ? '✓' : ''}
            <input type="file" accept="image/*" capture="user" onChange={(e) => {
              const file = e.target.files?.[0];
              if (file) selfieMutation.mutate(file);
            }} />
          </li>
          <li className="list-group-item d-flex justify-content-between align-items-center">
            GPS coordinates {visit.hasGps ? '✓' : ''}
            <button type="button" className="btn btn-sm btn-outline-primary" onClick={() => gpsMutation.mutate()}>
              Capture GPS
            </button>
          </li>
          <li className="list-group-item">
            Before / During / After photos {visit.hasBeforeDuringAfterPhoto ? '✓' : ''}
            <div className="d-flex flex-wrap gap-2 mt-2">
              {(['Before', 'During', 'After'] as const).map((category) => (
                <label key={category} className="btn btn-sm btn-outline-secondary mb-0">
                  {category}
                  <input type="file" accept="image/*" hidden onChange={(e) => {
                    const file = e.target.files?.[0];
                    if (file) photoMutation.mutate({ file, category });
                  }} />
                </label>
              ))}
            </div>
          </li>
          <li className="list-group-item">
            Customer signature {visit.hasSignature ? '✓' : ''}
            <input type="text" className="form-control form-control-sm mt-2" placeholder="Signer name" value={signerName} onChange={(e) => setSignerName(e.target.value)} />
            <canvas ref={canvasRef} width={320} height={120} className="border mt-2 w-100" />
            <button type="button" className="btn btn-sm btn-outline-primary mt-2" onClick={() => signatureMutation.mutate()}>
              Save signature
            </button>
          </li>
          <li className="list-group-item">
            Visit video evidence (optional, max 100 MB)
            <div className="mt-2">
              <input
                type="file"
                accept="video/mp4,video/quicktime"
                onChange={(e) => {
                  const file = e.target.files?.[0];
                  if (file) videoMutation.mutate(file);
                }}
              />
            </div>
            {videoAttachments.length > 0 && (
              <ul className="list-unstyled small mt-2 mb-0">
                {videoAttachments.map((attachment) => (
                  <li key={attachment.id} className="d-flex justify-content-between align-items-center">
                    <span>{attachment.title ?? attachment.fileId}</span>
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-secondary"
                      onClick={() =>
                        void visitsApi.downloadFile(attachment.fileId, attachment.title ?? 'visit-video.mp4')
                      }
                    >
                      Download
                    </button>
                  </li>
                ))}
              </ul>
            )}
          </li>
          <li className="list-group-item">
            Visit remarks {visit.hasMinimumRemarks ? '✓' : ''}
            <textarea className="form-control mt-2" rows={3} value={remarks} onChange={(e) => setRemarks(e.target.value)} placeholder="Minimum 10 characters required" />
            <button type="button" className="btn btn-sm btn-outline-primary mt-2" onClick={() => remarksMutation.mutate()}>
              Save remarks
            </button>
          </li>
        </ul>
      </div>

      <button
        type="button"
        className="btn btn-success"
        disabled={!readyToSubmit || submitMutation.isPending}
        onClick={() => submitMutation.mutate()}
      >
        Submit report for approval
      </button>
    </div>
  );
}
