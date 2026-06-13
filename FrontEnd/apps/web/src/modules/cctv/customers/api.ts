import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  ChangeCustomerStatusRequest,
  CreateCustomerRequest,
  CustomerDetail,
  CustomerListFilters,
  CustomerStatus,
  CustomerSummary,
  PagedCustomers,
  UpdateCustomerRequest,
} from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapCustomerSummary(raw: Record<string, unknown>): CustomerSummary {
  return {
    id: str(raw, 'id', 'Id'),
    customerNumber: str(raw, 'customerNumber', 'CustomerNumber'),
    name: str(raw, 'name', 'Name'),
    email: str(raw, 'email', 'Email'),
    phone: str(raw, 'phone', 'Phone'),
    city: str(raw, 'city', 'City'),
    status: str(raw, 'status', 'Status') as CustomerStatus,
    sourceLeadId: (raw.sourceLeadId ?? raw.SourceLeadId ?? null) as string | null,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapCustomerDetail(raw: Record<string, unknown>): CustomerDetail {
  const summary = mapCustomerSummary(raw);
  return {
    ...summary,
    billingAddress: str(raw, 'billingAddress', 'BillingAddress'),
    portalUserId: (raw.portalUserId ?? raw.PortalUserId ?? null) as string | null,
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
  };
}

function mapPagedCustomers(raw: Record<string, unknown>): PagedCustomers {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapCustomerSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

export const customersApi = {
  list: async (filters: CustomerListFilters = {}): Promise<PagedCustomers> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.status) params.set('status', filters.status);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.customers.list}?${qs}` : ENDPOINTS.cctv.customers.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedCustomers(res.data);
  },

  get: async (id: string): Promise<CustomerDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.customers.byId(id));
    return mapCustomerDetail(res.data);
  },

  create: async (body: CreateCustomerRequest): Promise<CustomerDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.customers.list, body);
    return mapCustomerDetail(res.data);
  },

  update: async (id: string, body: UpdateCustomerRequest): Promise<CustomerDetail> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.cctv.customers.byId(id), body);
    return mapCustomerDetail(res.data);
  },

  changeStatus: async (id: string, body: ChangeCustomerStatusRequest): Promise<CustomerDetail> => {
    const res = await apiClient.patch<Record<string, unknown>>(
      ENDPOINTS.cctv.customers.status(id),
      body,
    );
    return mapCustomerDetail(res.data);
  },
};
