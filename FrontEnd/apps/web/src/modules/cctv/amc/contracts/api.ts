import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { AmcContractDetail, AmcContractListFilters, ContractStatus, PagedAmcContracts } from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapContractSummary(raw: Record<string, unknown>) {
  return {
    id: str(raw, 'id', 'Id'),
    contractNumber: str(raw, 'contractNumber', 'ContractNumber'),
    siteId: str(raw, 'siteId', 'SiteId'),
    customerId: str(raw, 'customerId', 'CustomerId'),
    status: str(raw, 'status', 'Status') as ContractStatus,
    activeTermId: (raw.activeTermId ?? raw.ActiveTermId ?? null) as string | null,
    activeTermEndDate: (raw.activeTermEndDate ?? raw.ActiveTermEndDate ?? null) as string | null,
    planCode: (raw.planCode ?? raw.PlanCode ?? null) as string | null,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapContractDetail(raw: Record<string, unknown>): AmcContractDetail {
  const termsRaw = (raw.terms ?? raw.Terms ?? []) as unknown[];
  return {
    id: str(raw, 'id', 'Id'),
    contractNumber: str(raw, 'contractNumber', 'ContractNumber'),
    siteId: str(raw, 'siteId', 'SiteId'),
    customerId: str(raw, 'customerId', 'CustomerId'),
    sourceLeadId: (raw.sourceLeadId ?? raw.SourceLeadId ?? null) as string | null,
    status: str(raw, 'status', 'Status') as ContractStatus,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
    terms: termsRaw.map((t) => {
      const item = t as Record<string, unknown>;
      return {
        id: str(item, 'id', 'Id'),
        termNo: num(item, 'termNo', 'TermNo'),
        planVersionId: str(item, 'planVersionId', 'PlanVersionId'),
        planCode: str(item, 'planCode', 'PlanCode'),
        planVersionNo: num(item, 'planVersionNo', 'PlanVersionNo'),
        startDate: str(item, 'startDate', 'StartDate'),
        endDate: str(item, 'endDate', 'EndDate'),
        agreedPrice: num(item, 'agreedPrice', 'AgreedPrice'),
        status: str(item, 'status', 'Status') as AmcContractDetail['terms'][number]['status'],
        origin: str(item, 'origin', 'Origin') as AmcContractDetail['terms'][number]['origin'],
        renewalRequestedByCustomer: Boolean(item.renewalRequestedByCustomer ?? item.RenewalRequestedByCustomer),
        renewalRequestedAtUtc: (item.renewalRequestedAtUtc ?? item.RenewalRequestedAtUtc ?? null) as string | null,
        rowVersion: num(item, 'rowVersion', 'RowVersion'),
      };
    }),
  };
}

function mapPaged(raw: Record<string, unknown>): PagedAmcContracts {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapContractSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

export const amcContractsApi = {
  list: async (filters: AmcContractListFilters = {}): Promise<PagedAmcContracts> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.siteId) params.set('siteId', filters.siteId);
    if (filters.customerId) params.set('customerId', filters.customerId);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.contracts.list}?${qs}` : ENDPOINTS.cctv.contracts.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPaged(res.data);
  },

  get: async (id: string): Promise<AmcContractDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.contracts.byId(id));
    return mapContractDetail(res.data);
  },

  submitRenewalRequest: async (contractId: string, message?: string | null): Promise<void> => {
    await apiClient.post(ENDPOINTS.cctv.contracts.renewalRequest(contractId), { message: message ?? null });
  },
};
