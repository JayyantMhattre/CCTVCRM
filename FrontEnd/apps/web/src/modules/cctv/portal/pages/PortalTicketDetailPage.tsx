import { useState, type FormEvent } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { ticketsApi } from '../../tickets/api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { FileUpload } from '@/shared/file-upload';
import { ROUTES } from '@/core/router/routeMap';

export default function PortalTicketDetailPage() {
  const { ticketId = '' } = useParams<{ ticketId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();
  const [commentText, setCommentText] = useState('');
  const [reopenReason, setReopenReason] = useState('');

  const ticketQuery = useQuery({
    queryKey: ['cctv', 'tickets', ticketId],
    queryFn: () => ticketsApi.get(ticketId),
    enabled: Boolean(ticketId),
  });

  const commentMutation = useMutation({
    mutationFn: () => ticketsApi.addComment(ticketId, commentText.trim()),
    onSuccess: () => {
      toast.success('Comment added.');
      setCommentText('');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'tickets', ticketId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const reopenMutation = useMutation({
    mutationFn: () => ticketsApi.reopen(ticketId, reopenReason.trim(), ticketQuery.data!.rowVersion),
    onSuccess: () => {
      toast.success('Ticket reopened.');
      setReopenReason('');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'tickets', ticketId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const attachMutation = useMutation({
    mutationFn: (fileId: string) => ticketsApi.linkAttachment(ticketId, fileId),
    onSuccess: () => {
      toast.success('Attachment linked.');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'tickets', ticketId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (ticketQuery.isLoading) return <Spinner fullPage />;
  if (ticketQuery.error) return <AlertMessage message={extractMessage(ticketQuery.error)} variant="danger" />;

  const ticket = ticketQuery.data;
  if (!ticket) return null;

  const canReopen = ticket.status === 'Closed';

  function handleCommentSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!commentText.trim()) return;
    commentMutation.mutate();
  }

  return (
    <div>
      <PageHeader title={ticket.ticketNumber} subtitle={ticket.subject}>
        <Link to={ROUTES.cctv.portal.tickets} className="btn btn-outline-secondary btn-sm">
          Back to tickets
        </Link>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <span className="badge bg-secondary me-2">{ticket.status}</span>
              <span className="badge bg-warning text-dark">{ticket.priority}</span>
              <p className="mt-3 mb-0">{ticket.description}</p>
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">Comments</h2>
            </div>
            <ul className="list-group list-group-flush">
              {ticket.comments.length === 0 ? (
                <li className="list-group-item text-muted">No comments yet.</li>
              ) : (
                ticket.comments.map((comment) => (
                  <li key={comment.id} className="list-group-item">
                    <div className="small text-muted mb-1">
                      {comment.authorRole} · {comment.createdAtUtc}
                    </div>
                    <div>{comment.comment}</div>
                  </li>
                ))
              )}
            </ul>
            <div className="card-body border-top">
              <form onSubmit={handleCommentSubmit}>
                <textarea
                  className="form-control mb-2"
                  rows={3}
                  placeholder="Add a comment"
                  value={commentText}
                  onChange={(e) => setCommentText(e.target.value)}
                />
                <button type="submit" className="btn btn-sm btn-primary" disabled={commentMutation.isPending}>
                  Add comment
                </button>
              </form>
            </div>
          </div>

          <div className="card border-0 shadow-sm">
            <div className="card-header bg-white d-flex justify-content-between align-items-center">
              <h2 className="h6 mb-0">Attachments</h2>
              <FileUpload
                onUploaded={(file) => attachMutation.mutate(file.id)}
                disabled={attachMutation.isPending}
              />
            </div>
            <ul className="list-group list-group-flush">
              {ticket.attachments.length === 0 ? (
                <li className="list-group-item text-muted">No attachments.</li>
              ) : (
                ticket.attachments.map((attachment) => (
                  <li key={attachment.id} className="list-group-item small">
                    {attachment.title ?? attachment.fileId}
                  </li>
                ))
              )}
            </ul>
          </div>
        </div>

        <div className="col-lg-4">
          {canReopen && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-body">
                <h2 className="h6 fw-semibold">Reopen ticket</h2>
                <p className="small text-muted">Provide a reason (minimum 10 characters).</p>
                <textarea
                  className="form-control mb-2"
                  rows={3}
                  value={reopenReason}
                  onChange={(e) => setReopenReason(e.target.value)}
                />
                <button
                  type="button"
                  className="btn btn-sm btn-outline-primary w-100"
                  disabled={reopenMutation.isPending || reopenReason.trim().length < 10}
                  onClick={() => reopenMutation.mutate()}
                >
                  Reopen ticket
                </button>
              </div>
            </div>
          )}

          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <dl className="row mb-0 small">
                <dt className="col-5">Created</dt>
                <dd className="col-7">{ticket.createdAtUtc}</dd>
                <dt className="col-5">Reopens</dt>
                <dd className="col-7">{ticket.reopenCount}</dd>
                <dt className="col-5">Engineer</dt>
                <dd className="col-7">{ticket.assignedEngineerId ?? '—'}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
