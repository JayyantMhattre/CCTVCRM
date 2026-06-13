import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  ChangeEngineerStatusRequest,
  CreateEngineerRequest,
  EngineerDetail,
  EngineerListFilters,
  EngineerStatus,
  EngineerSummary,
  EngineerWorkload,
  PagedEngineers,
  UpdateEngineerRequest,
} from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapEngineerSummary(raw: Record<string, unknown>): EngineerSummary {
  return {
    id: str(raw, 'id', 'Id'),
    engineerNumber: str(raw, 'engineerNumber', 'EngineerNumber'),
    name: str(raw, 'name', 'Name'),
    phone: str(raw, 'phone', 'Phone'),
    status: str(raw, 'status', 'Status') as EngineerStatus,
    platformUserId: (raw.platformUserId ?? raw.PlatformUserId ?? null) as string | null,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapEngineerDetail(raw: Record<string, unknown>): EngineerDetail {
  const summary = mapEngineerSummary(raw);
  return {
    ...summary,
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
  };
}

function mapPagedEngineers(raw: Record<string, unknown>): PagedEngineers {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapEngineerSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

function mapWorkload(raw: Record<string, unknown>): EngineerWorkload {
  return {
    engineerId: str(raw, 'engineerId', 'EngineerId'),
    activeScheduleCount: num(raw, 'activeScheduleCount', 'ActiveScheduleCount'),
    openTicketCount: num(raw, 'openTicketCount', 'OpenTicketCount'),
  };
}

export const engineersApi = {
  list: async (filters: EngineerListFilters = {}): Promise<PagedEngineers> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.engineers.list}?${qs}` : ENDPOINTS.cctv.engineers.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedEngineers(res.data);
  },

  get: async (id: string): Promise<EngineerDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.engineers.byId(id));
    return mapEngineerDetail(res.data);
  },

  create: async (body: CreateEngineerRequest): Promise<EngineerDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.engineers.list, body);
    return mapEngineerDetail(res.data);
  },

  update: async (id: string, body: UpdateEngineerRequest): Promise<EngineerDetail> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.cctv.engineers.byId(id), body);
    return mapEngineerDetail(res.data);
  },

  changeStatus: async (id: string, body: ChangeEngineerStatusRequest): Promise<EngineerDetail> => {
    const res = await apiClient.patch<Record<string, unknown>>(
      ENDPOINTS.cctv.engineers.status(id),
      body,
    );
    return mapEngineerDetail(res.data);
  },

  workload: async (id: string): Promise<EngineerWorkload> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.engineers.workload(id));
    return mapWorkload(res.data);
  },
};
