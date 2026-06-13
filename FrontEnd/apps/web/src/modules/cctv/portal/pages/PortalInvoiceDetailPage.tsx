import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { invoicesApi } from '../../invoices/api';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { ROUTES } from '@/core/router/routeMap';

export default function PortalInvoiceDetailPage() {
  const { invoiceId = '' } = useParams<{ invoiceId: string }>();
  const { extractMessage } = useApiError();

  const invoiceQuery = useQuery({
    queryKey: ['cctv', 'invoices', invoiceId],
    queryFn: () => invoicesApi.get(invoiceId),
    enabled: Boolean(invoiceId),
  });

  const pdfQuery = useQuery({
    queryKey: ['cctv', 'invoices', invoiceId, 'pdf'],
    queryFn: () => invoicesApi.getPdf(invoiceId),
    enabled: Boolean(invoiceId) && invoiceQuery.data?.status !== 'Draft',
    retry: false,
  });

  if (invoiceQuery.isLoading) return <Spinner fullPage />;
  if (invoiceQuery.error) return <AlertMessage message={extractMessage(invoiceQuery.error)} variant="danger" />;

  const invoice = invoiceQuery.data;
  if (!invoice) return null;

  return (
    <div>
      <PageHeader title={invoice.invoiceNumber} subtitle={`${invoice.invoiceType} · ${invoice.status}`}>
        <Link to={ROUTES.cctv.portal.invoices} className="btn btn-outline-secondary btn-sm">
          Back to invoices
        </Link>
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
                  {invoice.lines.map((line) => (
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
                      Total
                    </td>
                    <td className="text-end fw-medium">{invoice.totalAmount.toFixed(2)}</td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <dl className="row mb-3 small">
                <dt className="col-5">Status</dt>
                <dd className="col-7">{invoice.status}</dd>
                <dt className="col-5">Invoice date</dt>
                <dd className="col-7">{invoice.invoiceDate}</dd>
                <dt className="col-5">Due date</dt>
                <dd className="col-7">{invoice.dueDate ?? '—'}</dd>
                <dt className="col-5">Amount</dt>
                <dd className="col-7">{invoice.totalAmount.toFixed(2)}</dd>
              </dl>
              {pdfQuery.data?.downloadUrl && (
                <a
                  href={pdfQuery.data.downloadUrl}
                  className="btn btn-primary btn-sm w-100"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  Download PDF
                </a>
              )}
              {pdfQuery.isError && (
                <div className="small text-muted">PDF not available for this invoice.</div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
