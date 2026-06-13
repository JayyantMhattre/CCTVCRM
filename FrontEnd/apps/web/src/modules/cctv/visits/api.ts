import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import { uploadFile } from '@/shared/file-upload';
import type { PagedVisits, VisitDetail, VisitListFilters, VisitSummary } from './types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function bool(raw: Record<string, unknown>, camel: string, pascal: string): boolean {
  return Boolean(raw[camel] ?? raw[pascal] ?? false);
}

function mapVisitSummary(raw: Record<string, unknown>): VisitSummary {
  return {
    id: str(raw, 'id', 'Id'),
    serviceScheduleId: str(raw, 'serviceScheduleId', 'ServiceScheduleId'),
    scheduleNumber: str(raw, 'scheduleNumber', 'ScheduleNumber'),
    siteId: str(raw, 'siteId', 'SiteId'),
    engineerId: str(raw, 'engineerId', 'EngineerId'),
    reportStatus: str(raw, 'reportStatus', 'ReportStatus') as VisitSummary['reportStatus'],
    startedAtUtc: (raw.startedAtUtc ?? raw.StartedAtUtc ?? null) as string | null,
    completedAtUtc: (raw.completedAtUtc ?? raw.CompletedAtUtc ?? null) as string | null,
    isCustomerVisible: bool(raw, 'isCustomerVisible', 'IsCustomerVisible'),
    createdAtUtc: str(raw, 'createdAtUtc', 'CreatedAtUtc'),
  };
}

function mapVisitDetail(raw: Record<string, unknown>): VisitDetail {
  const summary = mapVisitSummary(raw);
  const attachmentsRaw = (raw.attachments ?? raw.Attachments ?? []) as unknown[];
  return {
    ...summary,
    visitRemarks: (raw.visitRemarks ?? raw.VisitRemarks ?? null) as string | null,
    createdBy: str(raw, 'createdBy', 'CreatedBy'),
    updatedAtUtc: (raw.updatedAtUtc ?? raw.UpdatedAtUtc ?? null) as string | null,
    updatedBy: (raw.updatedBy ?? raw.UpdatedBy ?? null) as string | null,
    rowVersion: num(raw, 'rowVersion', 'RowVersion'),
    hasSelfie: bool(raw, 'hasSelfie', 'HasSelfie'),
    hasGps: bool(raw, 'hasGps', 'HasGps'),
    hasBeforeDuringAfterPhoto: bool(raw, 'hasBeforeDuringAfterPhoto', 'HasBeforeDuringAfterPhoto'),
    hasSignature: bool(raw, 'hasSignature', 'HasSignature'),
    hasMinimumRemarks: bool(raw, 'hasMinimumRemarks', 'HasMinimumRemarks'),
    attachments: attachmentsRaw.map((item) => {
      const row = item as Record<string, unknown>;
      return {
        id: str(row, 'id', 'Id'),
        fileId: str(row, 'fileId', 'FileId'),
        attachmentType: str(row, 'attachmentType', 'AttachmentType'),
        title: (row.title ?? row.Title ?? null) as string | null,
        createdAtUtc: str(row, 'createdAtUtc', 'CreatedAtUtc'),
        createdBy: str(row, 'createdBy', 'CreatedBy'),
      };
    }),
  };
}

function mapPagedVisits(raw: Record<string, unknown>): PagedVisits {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as unknown[];
  return {
    items: itemsRaw.map((item) => mapVisitSummary(item as Record<string, unknown>)),
    page: num(raw, 'page', 'Page') || num(raw, 'pageNumber', 'PageNumber') || 1,
    pageSize: num(raw, 'pageSize', 'PageSize') || 20,
    totalCount: num(raw, 'totalCount', 'TotalCount'),
    totalPages: num(raw, 'totalPages', 'TotalPages') || 1,
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  };
}

export const visitsApi = {
  list: async (filters: VisitListFilters = {}): Promise<PagedVisits> => {
    const params = new URLSearchParams();
    if (filters.page) params.set('page', String(filters.page));
    if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
    if (filters.reportStatus) params.set('reportStatus', filters.reportStatus);
    const qs = params.toString();
    const url = qs ? `${ENDPOINTS.cctv.visits.list}?${qs}` : ENDPOINTS.cctv.visits.list;
    const res = await apiClient.get<Record<string, unknown>>(url);
    return mapPagedVisits(res.data);
  },

  get: async (id: string): Promise<VisitDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.visits.byId(id));
    return mapVisitDetail(res.data);
  },

  getEngineerVisit: async (id: string): Promise<VisitDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.engineer.visitById(id));
    return mapVisitDetail(res.data);
  },

  start: async (id: string, rowVersion: number): Promise<VisitDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.start(id), { rowVersion });
    return mapVisitDetail(res.data);
  },

  updateRemarks: async (id: string, remarks: string, rowVersion: number): Promise<VisitDetail> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.cctv.visits.remarks(id), {
      remarks,
      rowVersion,
    });
    return mapVisitDetail(res.data);
  },

  linkPhoto: async (
    id: string,
    file: File,
    category: 'Before' | 'During' | 'After',
  ): Promise<VisitDetail> => {
    const uploaded = await uploadFile(file);
    const fileId = uploaded.id;
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.photos(id), {
      fileId,
      category,
      capturedAt: new Date().toISOString(),
    });
    return mapVisitDetail(res.data);
  },

  linkSelfie: async (id: string, file: File): Promise<VisitDetail> => {
    const uploaded = await uploadFile(file);
    const fileId = uploaded.id;
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.selfie(id), {
      fileId,
      capturedAt: new Date().toISOString(),
    });
    return mapVisitDetail(res.data);
  },

  captureLocation: async (id: string, latitude: number, longitude: number): Promise<VisitDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.location(id), {
      latitude,
      longitude,
      capturedAt: new Date().toISOString(),
    });
    return mapVisitDetail(res.data);
  },

  linkSignature: async (
    id: string,
    file: File,
    signerName: string,
  ): Promise<VisitDetail> => {
    const uploaded = await uploadFile(file);
    const fileId = uploaded.id;
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.signature(id), {
      fileId,
      signerName,
      capturedAt: new Date().toISOString(),
    });
    return mapVisitDetail(res.data);
  },

  submitReport: async (id: string, rowVersion: number): Promise<VisitDetail> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.submit(id), { rowVersion });
    return mapVisitDetail(res.data);
  },

  /** Visit video evidence — max 100 MB per LLD (BR-VISIT-06). */
  linkVideo: async (id: string, file: File, title?: string | null): Promise<VisitDetail> => {
    const maxBytes = 100 * 1024 * 1024;
    if (file.size > maxBytes) {
      throw new Error('Video exceeds maximum size (100 MB).');
    }
    const uploaded = await uploadFile(file);
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.cctv.visits.attachments(id), {
      fileId: uploaded.id,
      attachmentType: 'Video',
      title: title ?? file.name,
    });
    return mapVisitDetail(res.data);
  },

  downloadFile: async (fileId: string, fileName: string): Promise<void> => {
    const url = `${import.meta.env.VITE_API_VERSION ? `/api/${import.meta.env.VITE_API_VERSION}` : '/api/v1'}/files/${fileId}`;
    const response = await apiClient.get<Blob>(url, { responseType: 'blob' });
    const blob = new Blob([response.data]);
    const objectUrl = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = objectUrl;
    anchor.download = fileName;
    anchor.click();
    URL.revokeObjectURL(objectUrl);
  },

  listEngineer: async (): Promise<readonly VisitSummary[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.engineer.schedules);
    return (res.data ?? []).map((item) => mapVisitSummary(item as Record<string, unknown>));
  },
};
