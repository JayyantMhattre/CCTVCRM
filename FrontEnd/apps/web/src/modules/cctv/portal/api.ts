import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { AmcContractDetail } from '../amc/contracts/types';
import { amcContractsApi } from '../amc/contracts/api';
import type { PortalAmc, PortalScheduleSummary, PortalSiteSummary } from './types';
import type { VisitDetail, VisitSummary } from '../visits/types';

function str(raw: Record<string, unknown>, camel: string, pascal: string): string {
  return String(raw[camel] ?? raw[pascal] ?? '');
}

function num(raw: Record<string, unknown>, camel: string, pascal: string): number {
  return Number(raw[camel] ?? raw[pascal] ?? 0);
}

function mapPortalAmc(raw: Record<string, unknown>): PortalAmc {
  const servicesRaw = (raw.includedServices ?? raw.IncludedServices ?? []) as unknown[];
  return {
    contractId: str(raw, 'contractId', 'ContractId'),
    contractNumber: str(raw, 'contractNumber', 'ContractNumber'),
    siteId: str(raw, 'siteId', 'SiteId'),
    termId: str(raw, 'termId', 'TermId'),
    termNo: num(raw, 'termNo', 'TermNo'),
    planCode: str(raw, 'planCode', 'PlanCode'),
    planName: str(raw, 'planName', 'PlanName'),
    agreedPrice: num(raw, 'agreedPrice', 'AgreedPrice'),
    visitFrequencyPerYear: num(raw, 'visitFrequencyPerYear', 'VisitFrequencyPerYear'),
    includedServices: servicesRaw.map((s) => String(s)),
    slaTerms: str(raw, 'slaTerms', 'SlaTerms'),
    startDate: str(raw, 'startDate', 'StartDate'),
    endDate: str(raw, 'endDate', 'EndDate'),
    status: str(raw, 'status', 'Status'),
  };
}

function mapScheduleSummary(raw: Record<string, unknown>): PortalScheduleSummary {
  return {
    id: str(raw, 'id', 'Id'),
    scheduleNumber: str(raw, 'scheduleNumber', 'ScheduleNumber'),
    siteId: str(raw, 'siteId', 'SiteId'),
    scheduledDate: str(raw, 'scheduledDate', 'ScheduledDate'),
    status: str(raw, 'status', 'Status'),
    activeEngineerId: (raw.activeEngineerId ?? raw.ActiveEngineerId ?? null) as string | null,
    visitId: (raw.visitId ?? raw.VisitId ?? null) as string | null,
  };
}

function mapSiteSummary(raw: Record<string, unknown>): PortalSiteSummary {
  return {
    id: str(raw, 'id', 'Id'),
    siteNumber: str(raw, 'siteNumber', 'SiteNumber'),
    name: str(raw, 'name', 'Name'),
    address: str(raw, 'address', 'Address'),
    city: str(raw, 'city', 'City'),
    status: str(raw, 'status', 'Status'),
  };
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
    isCustomerVisible: Boolean(raw.isCustomerVisible ?? raw.IsCustomerVisible),
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
    hasSelfie: Boolean(raw.hasSelfie ?? raw.HasSelfie),
    hasGps: Boolean(raw.hasGps ?? raw.HasGps),
    hasBeforeDuringAfterPhoto: Boolean(raw.hasBeforeDuringAfterPhoto ?? raw.HasBeforeDuringAfterPhoto),
    hasSignature: Boolean(raw.hasSignature ?? raw.HasSignature),
    hasMinimumRemarks: Boolean(raw.hasMinimumRemarks ?? raw.HasMinimumRemarks),
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

export const portalApi = {
  getAmc: async (): Promise<PortalAmc | null> => {
    try {
      const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.portal.amc);
      return mapPortalAmc(res.data);
    } catch {
      return null;
    }
  },

  getContractHistory: async (contractId: string): Promise<AmcContractDetail> =>
    amcContractsApi.get(contractId),

  submitRenewalRequest: async (contractId: string, message?: string | null): Promise<void> => {
    await apiClient.post(ENDPOINTS.cctv.contracts.renewalRequest(contractId), { message: message ?? null });
  },

  listUpcomingVisits: async (): Promise<readonly PortalScheduleSummary[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.portalVisits.upcoming);
    return (res.data ?? []).map((item) => mapScheduleSummary(item as Record<string, unknown>));
  },

  listVisitHistory: async (): Promise<readonly VisitSummary[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.portalVisits.history);
    return (res.data ?? []).map((item) => mapVisitSummary(item as Record<string, unknown>));
  },

  getVisit: async (visitId: string): Promise<VisitDetail> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.cctv.portalVisits.byId(visitId));
    return mapVisitDetail(res.data);
  },

  listSites: async (): Promise<readonly PortalSiteSummary[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.cctv.portal.sites);
    return (res.data ?? []).map((item) => mapSiteSummary(item as Record<string, unknown>));
  },
};
