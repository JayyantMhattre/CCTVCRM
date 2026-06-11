/**
 * Audit module API functions.
 */

import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { AuditEntryDto, AuditLogFilters, AuditLogPage } from './types';

export const auditApi = {
  getLogs: async (filters?: AuditLogFilters): Promise<AuditLogPage> => {
    const res = await apiClient.get<AuditLogPage>(ENDPOINTS.audit.logs, { params: filters });
    const data = res.data;
    return {
      items: data.items.map(mapItem),
      page: data.page,
      pageSize: data.pageSize,
      totalCount: data.totalCount,
    };
  },
};

function mapItem(item: AuditLogPage['items'][number]): AuditEntryDto {
  return {
    id: item.id,
    tenantId: item.tenantId,
    userId: item.userId ?? null,
    module: item.module,
    action: item.action,
    eventType: item.eventType as AuditEntryDto['eventType'],
    ipAddress: item.ipAddress ?? null,
    userAgent: item.userAgent ?? null,
    oldValues: null,
    newValues: null,
    metadata: null,
    occurredOnUtc: item.occurredOnUtc,
  };
}
