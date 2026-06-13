import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  AmcPlanDetail,
  AmcPlanListFilters,
  AmcPlanVersionDetail,
  CreateAmcPlanRequest,
  CreateAmcPlanVersionRequest,
  PagedAmcPlans,
  PlanStatus,
  UpdateAmcPlanRequest,
} from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapPlanSummary(raw: Record<string, unknown>) {
  return {
    id: str(raw, 'id', 'Id'),
    planCode: str(raw, 'planCode', 'PlanCode'),
    name: str(raw, 'name', 'Name'),
    description: (raw.description ?? raw.Description ?? null) as string | null,
    status: str(raw, 'status', 'Status') as PlanStatus,
    publishedVersionCount: num(raw, 'publishedVersionCount', 'PublishedVersionCount'),
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapPlanDetail(raw: Record<string, unknown>): AmcPlanDetail {
  const versionsRaw = (raw.versions ?? raw.Versions ?? []) as unknown[];
  return {
    ...mapPlanSummary(raw),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
    versions: versionsRaw.map((v) => {
      const item = v as Record<string, unknown>;
      return {
        id: str(item, 'id', 'Id'),
        versionNo: num(item, 'versionNo', 'VersionNo'),
        price: num(item, 'price', 'Price'),
        visitFrequencyPerYear: num(item, 'visitFrequencyPerYear', 'VisitFrequencyPerYear'),
        effectiveFrom: str(item, 'effectiveFrom', 'EffectiveFrom'),
        status: str(item, 'status', 'Status') as AmcPlanDetail['versions'][number]['status'],
        createdAtUtc: str(item, 'createdAtUtc', 'CreatedAtUtc'),
      };
    }),
  };
}

function mapPaged(raw: Record<string, unknown>): PagedAmcPlans {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapPlanSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

export const amcPlansApi = {
  list: async (filters: AmcPlanListFilters = {}): Promise<PagedAmcPlans> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.amcPlans.list}?${qs}` : ENDPOINTS.cctv.amcPlans.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPaged(res.data);
  },

  get: async (id: string): Promise<AmcPlanDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.amcPlans.byId(id));
    return mapPlanDetail(res.data);
  },

  create: async (body: CreateAmcPlanRequest): Promise<AmcPlanDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.amcPlans.list, body);
    return mapPlanDetail(res.data);
  },

  update: async (id: string, body: UpdateAmcPlanRequest): Promise<AmcPlanDetail> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.cctv.amcPlans.byId(id), body);
    return mapPlanDetail(res.data);
  },

  retire: async (id: string, rowVersion: number): Promise<AmcPlanDetail> => {
    const res = await apiClient.patch<Record<string, unknown>>(ENDPOINTS.cctv.amcPlans.status(id), { rowVersion });
    return mapPlanDetail(res.data);
  },

  createVersion: async (planId: string, body: CreateAmcPlanVersionRequest): Promise<AmcPlanVersionDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.amcPlans.versions(planId), body);
    const raw = res.data;
    const services = (raw.includedServices ?? raw.IncludedServices ?? []) as string[];
    return {
      id: str(raw, 'id', 'Id'),
      planId: str(raw, 'planId', 'PlanId'),
      planCode: str(raw, 'planCode', 'PlanCode'),
      planName: str(raw, 'planName', 'PlanName'),
      versionNo: num(raw, 'versionNo', 'VersionNo'),
      price: num(raw, 'price', 'Price'),
      visitFrequencyPerYear: num(raw, 'visitFrequencyPerYear', 'VisitFrequencyPerYear'),
      includedServices: services,
      slaTerms: str(raw, 'slaTerms', 'SlaTerms'),
      effectiveFrom: str(raw, 'effectiveFrom', 'EffectiveFrom'),
      status: str(raw, 'status', 'Status') as AmcPlanVersionDetail['status'],
      isReferenced: Boolean(raw.isReferenced ?? raw.IsReferenced),
      createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
      createdBy: str(raw, 'createdBy', 'CreatedBy'),
    };
  },

  publishVersion: async (planId: string, versionId: string, rowVersion: number): Promise<AmcPlanVersionDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(
      ENDPOINTS.cctv.amcPlans.publishVersion(planId, versionId),
      { rowVersion },
    );
    const raw = res.data;
    const services = (raw.includedServices ?? raw.IncludedServices ?? []) as string[];
    return {
      id: str(raw, 'id', 'Id'),
      planId: str(raw, 'planId', 'PlanId'),
      planCode: str(raw, 'planCode', 'PlanCode'),
      planName: str(raw, 'planName', 'PlanName'),
      versionNo: num(raw, 'versionNo', 'VersionNo'),
      price: num(raw, 'price', 'Price'),
      visitFrequencyPerYear: num(raw, 'visitFrequencyPerYear', 'VisitFrequencyPerYear'),
      includedServices: services,
      slaTerms: str(raw, 'slaTerms', 'SlaTerms'),
      effectiveFrom: str(raw, 'effectiveFrom', 'EffectiveFrom'),
      status: str(raw, 'status', 'Status') as AmcPlanVersionDetail['status'],
      isReferenced: Boolean(raw.isReferenced ?? raw.IsReferenced),
      createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
      createdBy: str(raw, 'createdBy', 'CreatedBy'),
    };
  },
};
