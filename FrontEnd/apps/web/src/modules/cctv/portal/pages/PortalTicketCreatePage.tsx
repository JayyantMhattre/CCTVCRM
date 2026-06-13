import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useNavigate } from 'react-router-dom';
import { portalApi } from '../api';
import { ticketsApi } from '../../tickets/api';
import type { TicketPriority } from '../../tickets/types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { FileUpload } from '@/shared/file-upload';
import { ROUTES } from '@/core/router/routeMap';

const PRIORITIES: readonly TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];

export default function PortalTicketCreatePage() {
  const navigate = useNavigate();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();
  const [siteId, setSiteId] = useState('');
  const [subject, setSubject] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<TicketPriority>('Medium');
  const [attachmentFileIds, setAttachmentFileIds] = useState<string[]>([]);

  const sitesQuery = useQuery({
    queryKey: ['cctv', 'portal', 'sites'],
    queryFn: () => portalApi.listSites(),
  });

  const createMutation = useMutation({
    mutationFn: () =>
      ticketsApi.create({
        siteId,
        subject: subject.trim(),
        description: description.trim(),
        priority,
        attachmentFileIds,
      }),
    onSuccess: (ticket) => {
      toast.success('Ticket created.');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'portal', 'tickets'] });
      void navigate(ROUTES.cctv.portal.ticketDetail.replace(':ticketId', ticket.id));
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    createMutation.mutate();
  }

  if (sitesQuery.isLoading) return <Spinner fullPage />;
  if (sitesQuery.error) return <AlertMessage message={extractMessage(sitesQuery.error)} variant="danger" />;

  const sites = sitesQuery.data ?? [];

  return (
    <div>
      <PageHeader title="Create ticket" subtitle="Raise a support request for your site">
        <Link to={ROUTES.cctv.portal.tickets} className="btn btn-outline-secondary btn-sm">
          Back to tickets
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm">
        <div className="card-body p-4">
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label" htmlFor="site">
                Site *
              </label>
              <select
                id="site"
                className="form-select"
                required
                value={siteId}
                onChange={(e) => setSiteId(e.target.value)}
              >
                <option value="">Select site</option>
                {sites.map((site) => (
                  <option key={site.id} value={site.id}>
                    {site.name} — {site.city}
                  </option>
                ))}
              </select>
            </div>
            <div className="mb-3">
              <label className="form-label" htmlFor="subject">
                Subject *
              </label>
              <input
                id="subject"
                className="form-control"
                required
                value={subject}
                onChange={(e) => setSubject(e.target.value)}
              />
            </div>
            <div className="mb-3">
              <label className="form-label" htmlFor="description">
                Description *
              </label>
              <textarea
                id="description"
                className="form-control"
                rows={5}
                required
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </div>
            <div className="mb-3">
              <label className="form-label" htmlFor="priority">
                Priority
              </label>
              <select
                id="priority"
                className="form-select"
                value={priority}
                onChange={(e) => setPriority(e.target.value as TicketPriority)}
              >
                {PRIORITIES.map((p) => (
                  <option key={p} value={p}>
                    {p}
                  </option>
                ))}
              </select>
            </div>
            <div className="mb-3">
              <label className="form-label">Attachments</label>
              <FileUpload
                onUploaded={(file) => setAttachmentFileIds((prev) => [...prev, file.id])}
              />
              {attachmentFileIds.length > 0 && (
                <div className="small text-muted mt-2">{attachmentFileIds.length} file(s) attached</div>
              )}
            </div>
            <button type="submit" className="btn btn-primary" disabled={createMutation.isPending}>
              {createMutation.isPending ? 'Creating…' : 'Create ticket'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
