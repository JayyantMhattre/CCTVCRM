import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { PagedTickets, TicketDetail, TicketListFilters, TicketSummary } from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function bool(raw: Record<string, unknown>, camel: string, pascal: string): boolean {
  return Boolean(raw[camel] ?? raw[pascal] ?? false);
}

function mapTicketSummary(raw: Record<string, unknown>): TicketSummary {
  return {
    id: str(raw, 'id', 'Id'),
    ticketNumber: str(raw, 'ticketNumber', 'TicketNumber'),
    customerId: str(raw, 'customerId', 'CustomerId'),
    siteId: str(raw, 'siteId', 'SiteId'),
    subject: str(raw, 'subject', 'Subject'),
    priority: str(raw, 'priority', 'Priority') as TicketSummary['priority'],
    status: str(raw, 'status', 'Status') as TicketSummary['status'],
    assignedEngineerId: (raw.assignedEngineerId ?? raw.AssignedEngineerId ?? null) as string | null,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
    resolvedAtUtc: (raw.resolvedAtUtc ?? raw.ResolvedAtUtc ?? null) as string | null,
    closedAtUtc: (raw.closedAtUtc ?? raw.ClosedAtUtc ?? null) as string | null,
    reopenCount: num(raw, 'reopenCount', 'ReopenCount'),
  };
}

function mapTicketDetail(raw: Record<string, unknown>): TicketDetail {
  const summary = mapTicketSummary(raw);
  const commentsRaw = (raw.comments ?? raw.Comments ?? []) as unknown[];
  const attachmentsRaw = (raw.attachments ?? raw.Attachments ?? []) as unknown[];
  const historyRaw = (raw.statusHistory ?? raw.StatusHistory ?? []) as unknown[];

  return {
    ...summary,
    amcContractId: (raw.amcContractId ?? raw.AmcContractId ?? null) as string | null,
    originServiceVisitId: (raw.originServiceVisitId ?? raw.OriginServiceVisitId ?? null) as string | null,
    source: str(raw, 'source', 'Source') as TicketDetail['source'],
    description: str(raw, 'description', 'Description'),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
    comments: commentsRaw.map((c) => {
      const row = c as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        comment: str(row, 'comment', 'Comment'),
        authorRole: str(row, 'authorRole', 'AuthorRole'),
        createdAtUtc: str(row, 'createdAtUtc', 'CreatedAtUtc'),
        createdBy: str(row, 'createdBy', 'CreatedBy'),
      };
    }),
    attachments: attachmentsRaw.map((a) => {
      const row = a as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        fileId: str(row, 'fileId', 'FileId'),
        title: (row.title ?? row.Title ?? null) as string | null,
        createdAtUtc: str(row, 'createdAtUtc', 'CreatedAtUtc'),
        createdBy: str(row, 'createdBy', 'CreatedBy'),
      };
    }),
    statusHistory: historyRaw.map((h) => {
      const row = h as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        fromStatus: (row.fromStatus ?? row.FromStatus ?? null) as string | null,
        toStatus: str(row, 'toStatus', 'ToStatus'),
        reason: (row.reason ?? row.Reason ?? null) as string | null,
        changedAtUtc: str(row, 'changedAtUtc', 'ChangedAtUtc'),
        changedBy: str(row, 'changedBy', 'ChangedBy'),
      };
    }),
  };
}

function mapPagedTickets(raw: Record<string, unknown>): PagedTickets {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapTicketSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: bool(raw, 'hasNextPage', 'HasNextPage'),
    hasPreviousPage: bool(raw, 'hasPreviousPage', 'HasPreviousPage'),
  };
}

export const ticketsApi = {
  list: async (filters: TicketListFilters = {}): Promise<PagedTickets> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.priority) params.set('priority', filters.priority);
    if (filters.customerId) params.set('customerId', filters.customerId);
    if (filters.siteId) params.set('siteId', filters.siteId);
    if (filters.engineerId) params.set('engineerId', filters.engineerId);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.tickets.list}?${qs}` : ENDPOINTS.cctv.tickets.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedTickets(res.data);
  },

  get: async (id: string): Promise<TicketDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.tickets.byId(id));
    return mapTicketDetail(res.data);
  },

  listPortal: async (page = 1, pageSize = 20): Promise<PagedTickets> => {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    const res = await apiClient.get<Record<string, unknown>>(`${ENDPOINTS.cctv.tickets.portal}?${params}`);
    return mapPagedTickets(res.data);
  },

  listEngineer: async (page = 1, pageSize = 20): Promise<PagedTickets> => {
    const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
    const res = await apiClient.get<Record<string, unknown>>(`${ENDPOINTS.cctv.tickets.engineer}?${params}`);
    return mapPagedTickets(res.data);
  },

  updateStatus: async (
    id: string,
    body: { status: TicketDetail['status']; rowVersion: number; reason?: string | null },
  ): Promise<TicketDetail> => {
    const res = await apiClient.patch<Record<string, unknown>>(ENDPOINTS.cctv.tickets.status(id), body);
    return mapTicketDetail(res.data);
  },

  create: async (body: {
    siteId: string;
    subject: string;
    description: string;
    priority: TicketDetail['priority'];
    attachmentFileIds?: readonly string[];
  }): Promise<TicketDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.tickets.list, {
      siteId: body.siteId,
      subject: body.subject,
      description: body.description,
      priority: body.priority,
      attachmentFileIds: body.attachmentFileIds ?? null,
    });
    return mapTicketDetail(res.data);
  },

  addComment: async (id: string, text: string): Promise<TicketDetail> => {
    await apiClient.post(ENDPOINTS.cctv.tickets.comments(id), { text });
    return ticketsApi.get(id);
  },

  reopen: async (id: string, reason: string, rowVersion: number): Promise<TicketDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.tickets.reopen(id), {
      reason,
      rowVersion,
    });
    return mapTicketDetail(res.data);
  },

  linkAttachment: async (id: string, fileId: string, title?: string | null): Promise<TicketDetail> => {
    await apiClient.post(ENDPOINTS.cctv.tickets.attachments(id), { fileId, title: title ?? null });
    return ticketsApi.get(id);
  },
};
