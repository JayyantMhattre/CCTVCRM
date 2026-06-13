import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  CreateInvoiceRequest,
  InvoiceDetail,
  InvoiceListFilters,
  InvoicePdfInfo,
  InvoiceSummary,
  PagedInvoices,
  UpdateInvoiceDraftRequest,
} from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function bool(raw: Record<string, unknown>, camel: string, pascal: string): boolean {
  return Boolean(raw[camel] ?? raw[pascal] ?? false);
}

function mapInvoiceSummary(raw: Record<string, unknown>): InvoiceSummary {
  return {
    id: str(raw, 'id', 'Id'),
    invoiceNumber: str(raw, 'invoiceNumber', 'InvoiceNumber'),
    customerId: str(raw, 'customerId', 'CustomerId'),
    siteId: (raw.siteId ?? raw.SiteId ?? null) as string | null,
    invoiceType: str(raw, 'invoiceType', 'InvoiceType') as InvoiceSummary['invoiceType'],
    status: str(raw, 'status', 'Status') as InvoiceSummary['status'],
    invoiceDate: str(raw, 'invoiceDate', 'InvoiceDate'),
    dueDate: (raw.dueDate ?? raw.DueDate ?? null) as string | null,
    totalAmount: num(raw, 'totalAmount', 'TotalAmount'),
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
    paidAtUtc: (raw.paidAtUtc ?? raw.PaidAtUtc ?? null) as string | null,
  };
}

function mapInvoiceDetail(raw: Record<string, unknown>): InvoiceDetail {
  const summary = mapInvoiceSummary(raw);
  const linesRaw = (raw.lines ?? raw.Lines ?? []) as unknown[];
  const attachmentsRaw = (raw.attachments ?? raw.Attachments ?? []) as unknown[];
  const historyRaw = (raw.statusHistory ?? raw.StatusHistory ?? []) as unknown[];

  return {
    ...summary,
    amcContractTermId: (raw.amcContractTermId ?? raw.AmcContractTermId ?? null) as string | null,
    ticketId: (raw.ticketId ?? raw.TicketId ?? null) as string | null,
    serviceVisitId: (raw.serviceVisitId ?? raw.ServiceVisitId ?? null) as string | null,
    subtotalAmount: num(raw, 'subtotalAmount', 'SubtotalAmount'),
    taxAmount: num(raw, 'taxAmount', 'TaxAmount'),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
    lines: linesRaw.map((line) => {
      const row = line as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        lineNo: num(row, 'lineNo', 'LineNo'),
        description: str(row, 'description', 'Description'),
        quantity: num(row, 'quantity', 'Quantity'),
        unitPrice: num(row, 'unitPrice', 'UnitPrice'),
        lineTotal: num(row, 'lineTotal', 'LineTotal'),
      };
    }),
    attachments: attachmentsRaw.map((attachment) => {
      const row = attachment as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        fileId: str(row, 'fileId', 'FileId'),
        attachmentType: str(row, 'attachmentType', 'AttachmentType'),
        createdAtUtc: str(row, 'createdAtUtc', 'CreatedAtUtc'),
        createdBy: str(row, 'createdBy', 'CreatedBy'),
      };
    }),
    statusHistory: historyRaw.map((entry) => {
      const row = entry as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        fromStatus: (row.fromStatus ?? row.FromStatus ?? null) as string | null,
        toStatus: str(row, 'toStatus', 'ToStatus'),
        changedAtUtc: str(row, 'changedAtUtc', 'ChangedAtUtc'),
        changedBy: str(row, 'changedBy', 'ChangedBy'),
      };
    }),
  };
}

function mapPagedInvoices(raw: Record<string, unknown>): PagedInvoices {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapInvoiceSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: bool(raw, 'hasNextPage', 'HasNextPage'),
    hasPreviousPage: bool(raw, 'hasPreviousPage', 'HasPreviousPage'),
  };
}

export const invoicesApi = {
  list: async (filters: InvoiceListFilters = {}): Promise<PagedInvoices> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.invoiceType) params.set('invoiceType', filters.invoiceType);
    if (filters.customerId) params.set('customerId', filters.customerId);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.invoices.list}?${qs}` : ENDPOINTS.cctv.invoices.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedInvoices(res.data);
  },

  get: async (id: string): Promise<InvoiceDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.invoices.byId(id));
    return mapInvoiceDetail(res.data);
  },

  create: async (payload: CreateInvoiceRequest): Promise<InvoiceDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.invoices.list, payload);
    return mapInvoiceDetail(res.data);
  },

  update: async (id: string, payload: UpdateInvoiceDraftRequest): Promise<InvoiceDetail> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.cctv.invoices.byId(id), payload);
    return mapInvoiceDetail(res.data);
  },

  generate: async (id: string, rowVersion: number): Promise<InvoiceDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.invoices.generate(id), {
      rowVersion,
    });
    return mapInvoiceDetail(res.data);
  },

  send: async (id: string, rowVersion: number): Promise<InvoiceDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.invoices.send(id), {
      rowVersion,
    });
    return mapInvoiceDetail(res.data);
  },

  markPaid: async (id: string, rowVersion: number): Promise<InvoiceDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.invoices.markPaid(id), {
      rowVersion,
    });
    return mapInvoiceDetail(res.data);
  },

  cancel: async (id: string, rowVersion: number): Promise<InvoiceDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.invoices.cancel(id), {
      rowVersion,
    });
    return mapInvoiceDetail(res.data);
  },

  getPdf: async (id: string): Promise<InvoicePdfInfo> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.invoices.pdf(id));
    const raw = res.data;
    return {
      invoiceId: str(raw, 'invoiceId', 'InvoiceId'),
      invoiceNumber: str(raw, 'invoiceNumber', 'InvoiceNumber'),
      fileId: str(raw, 'fileId', 'FileId'),
      downloadUrl: str(raw, 'downloadUrl', 'DownloadUrl'),
    };
  },

  listPortal: async (page = 1, pageSize = 20): Promise<PagedInvoices> => {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    const res = await apiClient.get<Record<string, unknown>>(`${ENDPOINTS.cctv.invoices.portal}?${params}`);
    return mapPagedInvoices(res.data);
  },
};
