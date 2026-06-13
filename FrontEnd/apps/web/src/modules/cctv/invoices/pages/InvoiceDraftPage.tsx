import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { invoicesApi } from '../api';
import type { InvoiceLineRequest, InvoiceType } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { ROUTES } from '@/core/router/routeMap';

const TYPE_OPTIONS: readonly InvoiceType[] = [
  'AmcRenewal',
  'NewAmc',
  'EmergencyService',
  'SpareReplacement',
  'AdditionalCharges',
  'Other',
];

const emptyLine = (): InvoiceLineRequest => ({
  description: '',
  quantity: 1,
  unitPrice: 0,
});

function invoiceDetailPath(invoiceId: string): string {
  return ROUTES.cctv.admin.invoiceDetail.replace(':invoiceId', invoiceId);
}

export default function InvoiceDraftPage() {
  const { invoiceId } = useParams<{ invoiceId?: string }>();
  const isEdit = Boolean(invoiceId);
  const navigate = useNavigate();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();

  const [customerId, setCustomerId] = useState('');
  const [invoiceType, setInvoiceType] = useState<InvoiceType>('Other');
  const [invoiceDate, setInvoiceDate] = useState(new Date().toISOString().slice(0, 10));
  const [dueDate, setDueDate] = useState('');
  const [taxAmount, setTaxAmount] = useState('0');
  const [lines, setLines] = useState<InvoiceLineRequest[]>([emptyLine()]);
  const [rowVersion, setRowVersion] = useState(0);

  const existingQuery = useQuery({
    queryKey: ['cctv', 'invoices', invoiceId],
    queryFn: () => invoicesApi.get(invoiceId!),
    enabled: isEdit,
  });

  useEffect(() => {
    if (!existingQuery.data) return;
    const invoice = existingQuery.data;
    setCustomerId(invoice.customerId);
    setInvoiceType(invoice.invoiceType);
    setInvoiceDate(invoice.invoiceDate);
    setDueDate(invoice.dueDate ?? '');
    setTaxAmount(String(invoice.taxAmount));
    setRowVersion(invoice.rowVersion);
    setLines(
      invoice.lines.map((line) => ({
        description: line.description,
        quantity: line.quantity,
        unitPrice: line.unitPrice,
      })),
    );
  }, [existingQuery.data]);

  const saveMutation = useMutation({
    mutationFn: async () => {
      const payloadLines = lines.filter((line) => line.description.trim());
      if (payloadLines.length === 0) throw new Error('At least one line item is required.');

      if (isEdit) {
        return invoicesApi.update(invoiceId!, {
          invoiceType,
          invoiceDate,
          dueDate: dueDate || null,
          lines: payloadLines,
          taxAmount: Number(taxAmount) || 0,
          rowVersion,
        });
      }

      if (!customerId.trim()) throw new Error('Customer ID is required.');
      return invoicesApi.create({
        customerId: customerId.trim(),
        invoiceType,
        invoiceDate,
        dueDate: dueDate || null,
        lines: payloadLines,
        taxAmount: Number(taxAmount) || 0,
      });
    },
    onSuccess: (invoice) => {
      toast.success(isEdit ? 'Draft updated' : 'Draft created');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'invoices'] });
      void navigate(invoiceDetailPath(invoice.id));
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const generateMutation = useMutation({
    mutationFn: () => invoicesApi.generate(invoiceId!, rowVersion),
    onSuccess: (invoice) => {
      toast.success('Invoice generated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'invoices'] });
      void navigate(invoiceDetailPath(invoice.id));
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (isEdit && existingQuery.isLoading) return <Spinner fullPage />;
  if (isEdit && existingQuery.error) {
    return <AlertMessage message={extractMessage(existingQuery.error)} variant="danger" />;
  }

  const subtotal = lines.reduce((sum, line) => sum + line.quantity * line.unitPrice, 0);
  const total = subtotal + (Number(taxAmount) || 0);

  return (
    <div>
      <PageHeader
        title={isEdit ? 'Edit draft invoice' : 'New draft invoice'}
        subtitle={isEdit ? existingQuery.data?.invoiceNumber : 'Create invoice draft'}
      >
        <Link to={ROUTES.cctv.admin.invoices} className="btn btn-outline-secondary btn-sm">
          Back
        </Link>
      </PageHeader>

      <div className="card border-0 shadow-sm mb-3">
        <div className="card-body">
          {!isEdit && (
            <div className="mb-3">
              <label className="form-label">Customer ID</label>
              <input
                className="form-control font-monospace"
                value={customerId}
                onChange={(e) => setCustomerId(e.target.value)}
                placeholder="Customer UUID"
              />
            </div>
          )}
          <div className="row g-3 mb-3">
            <div className="col-md-4">
              <label className="form-label">Type</label>
              <select
                className="form-select"
                value={invoiceType}
                onChange={(e) => setInvoiceType(e.target.value as InvoiceType)}
              >
                {TYPE_OPTIONS.map((type) => (
                  <option key={type} value={type}>
                    {type}
                  </option>
                ))}
              </select>
            </div>
            <div className="col-md-4">
              <label className="form-label">Invoice date</label>
              <input
                type="date"
                className="form-control"
                value={invoiceDate}
                onChange={(e) => setInvoiceDate(e.target.value)}
              />
            </div>
            <div className="col-md-4">
              <label className="form-label">Due date</label>
              <input
                type="date"
                className="form-control"
                value={dueDate}
                onChange={(e) => setDueDate(e.target.value)}
              />
            </div>
          </div>

          <h2 className="h6">Line items</h2>
          {lines.map((line, index) => (
            <div key={index} className="row g-2 mb-2 align-items-end">
              <div className="col-md-6">
                <input
                  className="form-control"
                  placeholder="Description"
                  value={line.description}
                  onChange={(e) => {
                    const next = [...lines];
                    next[index] = { ...line, description: e.target.value };
                    setLines(next);
                  }}
                />
              </div>
              <div className="col-md-2">
                <input
                  type="number"
                  min={0}
                  step="0.01"
                  className="form-control"
                  placeholder="Qty"
                  value={line.quantity}
                  onChange={(e) => {
                    const next = [...lines];
                    next[index] = { ...line, quantity: Number(e.target.value) };
                    setLines(next);
                  }}
                />
              </div>
              <div className="col-md-2">
                <input
                  type="number"
                  min={0}
                  step="0.01"
                  className="form-control"
                  placeholder="Unit price"
                  value={line.unitPrice}
                  onChange={(e) => {
                    const next = [...lines];
                    next[index] = { ...line, unitPrice: Number(e.target.value) };
                    setLines(next);
                  }}
                />
              </div>
              <div className="col-md-2">
                <button
                  type="button"
                  className="btn btn-outline-danger btn-sm w-100"
                  disabled={lines.length <= 1}
                  onClick={() => setLines(lines.filter((_, i) => i !== index))}
                >
                  Remove
                </button>
              </div>
            </div>
          ))}
          <button type="button" className="btn btn-sm btn-outline-secondary mb-3" onClick={() => setLines([...lines, emptyLine()])}>
            Add line
          </button>

          <div className="row g-3">
            <div className="col-md-4">
              <label className="form-label">Tax amount</label>
              <input
                type="number"
                min={0}
                step="0.01"
                className="form-control"
                value={taxAmount}
                onChange={(e) => setTaxAmount(e.target.value)}
              />
            </div>
            <div className="col-md-8 d-flex align-items-end justify-content-end">
              <div className="text-end">
                <div className="text-muted small">Subtotal: {subtotal.toFixed(2)}</div>
                <div className="fw-semibold">Total: {total.toFixed(2)}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="d-flex flex-wrap gap-2">
        <button
          type="button"
          className="btn btn-primary"
          disabled={saveMutation.isPending}
          onClick={() => saveMutation.mutate()}
        >
          {isEdit ? 'Save draft' : 'Create draft'}
        </button>
        {isEdit && existingQuery.data?.status === 'Draft' && (
          <>
            <Link to={invoiceDetailPath(invoiceId!)} className="btn btn-outline-secondary">
              Preview
            </Link>
            <button
              type="button"
              className="btn btn-success"
              disabled={generateMutation.isPending}
              onClick={() => generateMutation.mutate()}
            >
              Generate invoice
            </button>
          </>
        )}
      </div>
    </div>
  );
}
