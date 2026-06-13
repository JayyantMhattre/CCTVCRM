import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  ChangeLeadStatusRequest,
  CreateLeadRemarkRequest,
  LeadActivity,
  LeadDetail,
  LeadListFilters,
  LeadRemark,
  LeadStatus,
  LeadSummary,
  PagedLeads,
} from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapLeadSummary(raw: Record<string, unknown>): LeadSummary {
  return {
    id: str(raw, 'id', 'Id'),
    leadNumber: str(raw, 'leadNumber', 'LeadNumber'),
    status: str(raw, 'status', 'Status') as LeadSummary['status'],
    source: str(raw, 'source', 'Source') as LeadSummary['source'],
    contactName: str(raw, 'contactName', 'ContactName'),
    organizationName: (raw.organizationName ?? raw.OrganizationName ?? null) as string | null,
    email: str(raw, 'email', 'Email'),
    phone: str(raw, 'phone', 'Phone'),
    city: str(raw, 'city', 'City'),
    ownerUserId: (raw.ownerUserId ?? raw.OwnerUserId ?? null) as string | null,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapLeadDetail(raw: Record<string, unknown>): LeadDetail {
  const summary = mapLeadSummary(raw);
  return {
    ...summary,
    address: (raw.address ?? raw.Address ?? null) as string | null,
    requirementSummary: (raw.requirementSummary ?? raw.RequirementSummary ?? null) as string | null,
    convertedCustomerId: (raw.convertedCustomerId ?? raw.ConvertedCustomerId ?? null) as string | null,
    convertedSiteId: (raw.convertedSiteId ?? raw.ConvertedSiteId ?? null) as string | null,
    convertedContractId: (raw.convertedContractId ?? raw.ConvertedContractId ?? null) as string | null,
    convertedAtUtc: (raw.convertedAtUtc ?? raw.ConvertedAtUtc ?? null) as string | null,
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
    activityCount: num(raw, 'activityCount', 'ActivityCount'),
    remarkCount: num(raw, 'remarkCount', 'RemarkCount'),
    attachmentCount: num(raw, 'attachmentCount', 'AttachmentCount'),
  };
}

function mapPagedLeads(raw: Record<string, unknown>): PagedLeads {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapLeadSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

function mapActivity(raw: Record<string, unknown>): LeadActivity {
  return {
    id: str(raw, 'id', 'Id'),
    activityType: str(raw, 'activityType', 'ActivityType'),
    fromStatus: (raw.fromStatus ?? raw.FromStatus ?? null) as LeadStatus | null,
    toStatus: (raw.toStatus ?? raw.ToStatus ?? null) as LeadStatus | null,
    description: str(raw, 'description', 'Description'),
    occurredAtUtc: str(raw, 'occurredAtUtc', 'OccurredAtUtc'),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
  };
}

function mapRemark(raw: Record<string, unknown>): LeadRemark {
  return {
    id: str(raw, 'id', 'Id'),
    text: str(raw, 'text', 'Text') || str(raw, 'remark', 'Remark'),
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
  };
}

export const leadsApi = {
  list: async (filters: LeadListFilters = {}): Promise<PagedLeads> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.leads.list}?${qs}` : ENDPOINTS.cctv.leads.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedLeads(res.data);
  },

  get: async (id: string): Promise<LeadDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.leads.byId(id));
    return mapLeadDetail(res.data);
  },

  changeStatus: async (id: string, body: ChangeLeadStatusRequest): Promise<LeadDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(
      ENDPOINTS.cctv.leads.status(id),
      body,
    );
    return mapLeadDetail(res.data);
  },

  getActivities: async (id: string): Promise<readonly LeadActivity[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.leads.activities(id));
    return (res.data ?? []).map((item) => mapActivity(item as Record<string, unknown>));
  },

  getRemarks: async (id: string): Promise<readonly LeadRemark[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.leads.remarks(id));
    return (res.data ?? []).map((item) => mapRemark(item as Record<string, unknown>));
  },

  addRemark: async (id: string, body: CreateLeadRemarkRequest): Promise<LeadRemark> => {
    const res = await apiClient.post<Record<string, unknown>>(
      ENDPOINTS.cctv.leads.remarks(id),
      body,
    );
    return mapRemark(res.data);
  },
};
