import { useParams, Link } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { invoicesApi } from '../api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

function invoiceEditPath(invoiceId: string): string {
  return ROUTES.cctv.admin.invoiceEdit.replace(':invoiceId', invoiceId);
}

export default function InvoiceDetailPage() {
  const { invoiceId = '' } = useParams<{ invoiceId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();

  const { data, isLoading, error } = useQuery({
    queryKey: ['cctv', 'invoices', invoiceId],
    queryFn: () => invoicesApi.get(invoiceId),
    enabled: Boolean(invoiceId),
  });

  const pdfQuery = useQuery({
    queryKey: ['cctv', 'invoices', invoiceId, 'pdf'],
    queryFn: () => invoicesApi.getPdf(invoiceId),
    enabled: Boolean(invoiceId) && data?.status !== 'Draft',
    retry: false,
  });

  const generateMutation = useMutation({
    mutationFn: () => invoicesApi.generate(invoiceId, data!.rowVersion),
    onSuccess: () => {
      toast.success('Invoice generated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'invoices', invoiceId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const sendMutation = useMutation({
    mutationFn: () => invoicesApi.send(invoiceId, data!.rowVersion),
    onSuccess: () => {
      toast.success('Invoice sent');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'invoices', invoiceId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const markPaidMutation = useMutation({
    mutationFn: () => invoicesApi.markPaid(invoiceId, data!.rowVersion),
    onSuccess: () => {
      toast.success('Invoice marked paid');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'invoices', invoiceId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const cancelMutation = useMutation({
    mutationFn: () => invoicesApi.cancel(invoiceId, data!.rowVersion),
    onSuccess: () => {
      toast.success('Invoice cancelled');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'invoices', invoiceId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (isLoading) return <Spinner fullPage />;

  if (error) {
    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;
  }

  if (!data) return null;

  return (
    <div>
      <PageHeader title={data.invoiceNumber} subtitle={`${data.invoiceType} · ${data.status}`}>
        <div className="d-flex gap-2">
          <Link to={ROUTES.cctv.admin.invoices} className="btn btn-outline-secondary btn-sm">
            Back
          </Link>
          {data.status === 'Draft' && (
            <>
              <Link to={invoiceEditPath(invoiceId)} className="btn btn-outline-primary btn-sm">
                Edit draft
              </Link>
              <button
                type="button"
                className="btn btn-success btn-sm"
                disabled={generateMutation.isPending}
                onClick={() => generateMutation.mutate()}
              >
                Generate
              </button>
            </>
          )}
          {data.status === 'Generated' && (
            <>
              <button
                type="button"
                className="btn btn-primary btn-sm"
                disabled={sendMutation.isPending}
                onClick={() => sendMutation.mutate()}
              >
                Send invoice
              </button>
              <button
                type="button"
                className="btn btn-outline-danger btn-sm"
                disabled={cancelMutation.isPending}
                onClick={() => cancelMutation.mutate()}
              >
                Cancel
              </button>
            </>
          )}
          {data.status === 'Sent' && (
            <>
              <button
                type="button"
                className="btn btn-success btn-sm"
                disabled={markPaidMutation.isPending}
                onClick={() => markPaidMutation.mutate()}
              >
                Mark paid
              </button>
              <button
                type="button"
                className="btn btn-outline-danger btn-sm"
                disabled={cancelMutation.isPending}
                onClick={() => cancelMutation.mutate()}
              >
                Cancel
              </button>
            </>
          )}
        </div>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">Line items</h2>
            </div>
            <div className="table-responsive">
              <table className="table mb-0 align-middle">
                <thead className="table-light">
                  <tr>
                    <th>#</th>
                    <th>Description</th>
                    <th className="text-end">Qty</th>
                    <th className="text-end">Unit price</th>
                    <th className="text-end">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {data.lines.map((line) => (
                    <tr key={line.id}>
                      <td>{line.lineNo}</td>
                      <td>{line.description}</td>
                      <td className="text-end">{line.quantity}</td>
                      <td className="text-end">{line.unitPrice.toFixed(2)}</td>
                      <td className="text-end">{line.lineTotal.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <td colSpan={4} className="text-end text-muted">
                      Subtotal
                    </td>
                    <td className="text-end">{data.subtotalAmount.toFixed(2)}</td>
                  </tr>
                  <tr>
                    <td colSpan={4} className="text-end text-muted">
                      Tax
                    </td>
                    <td className="text-end">{data.taxAmount.toFixed(2)}</td>
                  </tr>
                  <tr>
                    <td colSpan={4} className="text-end fw-medium">
                      Total
                    </td>
                    <td className="text-end fw-medium">{data.totalAmount.toFixed(2)}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white">
              <h2 className="h6 mb-0">Status history</h2>
            </div>
            <ul className="list-group list-group-flush">
              {data.statusHistory.length === 0 ? (
                <li className="list-group-item text-muted">No transitions recorded.</li>
              ) : (
                data.statusHistory.map((entry) => (
                  <li key={entry.id} className="list-group-item small">
                    {entry.fromStatus ?? '—'} → {entry.toStatus} · {entry.changedAtUtc}
                  </li>
                ))
              )}
            </ul>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <dl className="row mb-0 small">
                <dt className="col-5">Status</dt>
                <dd className="col-7">{data.status}</dd>
                <dt className="col-5">Invoice date</dt>
                <dd className="col-7">{data.invoiceDate}</dd>
                <dt className="col-5">Due date</dt>
                <dd className="col-7">{data.dueDate ?? '—'}</dd>
                <dt className="col-5">Customer</dt>
                <dd className="col-7 text-truncate">{data.customerId}</dd>
                <dt className="col-5">AMC term</dt>
                <dd className="col-7 text-truncate">{data.amcContractTermId ?? '—'}</dd>
              </dl>
            </div>
          </div>

          {pdfQuery.data && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-body">
                <h2 className="h6">Invoice PDF</h2>
                <a href={pdfQuery.data.downloadUrl} className="btn btn-sm btn-outline-primary" target="_blank" rel="noreferrer">
                  Download PDF
                </a>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
