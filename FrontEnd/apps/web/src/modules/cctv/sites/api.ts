import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  ChangeSiteStatusRequest,
  CreateSiteRequest,
  PagedSites,
  SiteAssetSummary,
  SiteContact,
  SiteDetail,
  SiteListFilters,
  SiteStatus,
  SiteSummary,
  UpdateSiteAssetSummaryRequest,
  UpdateSiteRequest,
  UpsertSiteContactsRequest,
  SiteDocument,
  SiteDocumentType,
} from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapSiteSummary(raw: Record<string, unknown>): SiteSummary {
  return {
    id: str(raw, 'id', 'Id'),
    siteNumber: str(raw, 'siteNumber', 'SiteNumber'),
    customerId: str(raw, 'customerId', 'CustomerId'),
    name: str(raw, 'name', 'Name'),
    address: str(raw, 'address', 'Address'),
    city: str(raw, 'city', 'City'),
    status: str(raw, 'status', 'Status') as SiteStatus,
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapSiteDetail(raw: Record<string, unknown>): SiteDetail {
  const summary = mapSiteSummary(raw);
  return {
    ...summary,
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
  };
}

function mapSiteContact(raw: Record<string, unknown>): SiteContact {
  return {
    id: str(raw, 'id', 'Id'),
    contactSlot: num(raw, 'contactSlot', 'ContactSlot'),
    name: str(raw, 'name', 'Name'),
    designation: (raw.designation ?? raw.Designation ?? null) as string | null,
    phone: str(raw, 'phone', 'Phone'),
    email: (raw.email ?? raw.Email ?? null) as string | null,
    isPrimary: Boolean(raw.isPrimary ?? raw.IsPrimary),
  };
}

function mapSiteDocument(raw: Record<string, unknown>): SiteDocument {
  return {
    id: str(raw, 'id', 'Id'),
    fileId: str(raw, 'fileId', 'FileId'),
    documentType: str(raw, 'documentType', 'DocumentType') as SiteDocumentType,
    title: str(raw, 'title', 'Title'),
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
  };
}

function mapSiteAssetSummary(raw: Record<string, unknown>): SiteAssetSummary {
  return {
    id: str(raw, 'id', 'Id'),
    cameraCount: num(raw, 'cameraCount', 'CameraCount'),
    dvrCount: num(raw, 'dvrCount', 'DvrCount'),
    nvrCount: num(raw, 'nvrCount', 'NvrCount'),
    hardDiskCount: num(raw, 'hardDiskCount', 'HardDiskCount'),
    switchCount: num(raw, 'switchCount', 'SwitchCount'),
    routerCount: num(raw, 'routerCount', 'RouterCount'),
    monitorCount: num(raw, 'monitorCount', 'MonitorCount'),
    brand: (raw.brand ?? raw.Brand ?? null) as string | null,
    model: (raw.model ?? raw.Model ?? null) as string | null,
    remarks: (raw.remarks ?? raw.Remarks ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
  };
}

function mapPagedSites(raw: Record<string, unknown>): PagedSites {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapSiteSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

export const sitesApi = {
  list: async (filters: SiteListFilters = {}): Promise<PagedSites> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.customerId) params.set('customerId', filters.customerId);
    if (filters.status) params.set('status', filters.status);
    if (filters.search) params.set('search', filters.search);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.sites.list}?${qs}` : ENDPOINTS.cctv.sites.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedSites(res.data);
  },

  get: async (id: string): Promise<SiteDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.sites.byId(id));
    return mapSiteDetail(res.data);
  },

  create: async (body: CreateSiteRequest): Promise<SiteDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.sites.list, body);
    return mapSiteDetail(res.data);
  },

  update: async (id: string, body: UpdateSiteRequest): Promise<SiteDetail> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.cctv.sites.byId(id), body);
    return mapSiteDetail(res.data);
  },

  changeStatus: async (id: string, body: ChangeSiteStatusRequest): Promise<SiteDetail> => {
    const res = await apiClient.patch<Record<string, unknown>>(ENDPOINTS.cctv.sites.status(id), body);
    return mapSiteDetail(res.data);
  },

  getContacts: async (id: string): Promise<readonly SiteContact[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.sites.contacts(id));
    return (res.data ?? []).map((item) => mapSiteContact(item as Record<string, unknown>));
  },

  upsertContacts: async (id: string, body: UpsertSiteContactsRequest): Promise<readonly SiteContact[]> => {
    const res = await apiClient.put<unknown[]>(ENDPOINTS.cctv.sites.contacts(id), body);
    return (res.data ?? []).map((item) => mapSiteContact(item as Record<string, unknown>));
  },

  getAssetSummary: async (id: string): Promise<SiteAssetSummary | null> => {
    try {
      const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.sites.assetSummary(id));
      return mapSiteAssetSummary(res.data);
    } catch {
      return null;
    }
  },

  updateAssetSummary: async (
    id: string,
    body: UpdateSiteAssetSummaryRequest,
  ): Promise<SiteAssetSummary> => {
    const res = await apiClient.put<Record<string, unknown>>(
      ENDPOINTS.cctv.sites.assetSummary(id),
      body,
    );
    return mapSiteAssetSummary(res.data);
  },

  getDocuments: async (id: string): Promise<readonly SiteDocument[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.sites.documents(id));
    return (res.data ?? []).map((item) => mapSiteDocument(item as Record<string, unknown>));
  },

  linkDocument: async (
    id: string,
    body: { fileId: string; documentType: SiteDocumentType; title: string; rowVersion: number },
  ): Promise<SiteDocument> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.sites.documents(id), body);
    return mapSiteDocument(res.data);
  },

  removeDocument: async (siteId: string, documentId: string, rowVersion: number): Promise<void> => {
    await apiClient.delete(ENDPOINTS.cctv.sites.document(siteId, documentId), {
      data: { rowVersion },
    });
  },
};
