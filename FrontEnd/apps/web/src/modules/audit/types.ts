import type { IsoDate } from '@/shared/types/api.types';

export type AuditEventType =
  | 'ApiCall'
  | 'EntityChange'
  | 'UserAction'
  | 'DomainEvent';

export interface AuditEntryDto {
  id:           string;
  tenantId:     string;
  userId:       string | null;
  module:       string;
  action:       string;
  eventType:    AuditEventType;
  ipAddress:    string | null;
  userAgent:    string | null;
  oldValues:    Record<string, unknown> | null;
  newValues:    Record<string, unknown> | null;
  metadata:     Record<string, unknown> | null;
  occurredOnUtc: IsoDate;
}

export interface AuditLogFilters {
  tenantId?:  string;
  module?:    string;
  search?:    string;
  from?:      string;
  to?:        string;
  page?:      number;
  pageSize?:  number;
}

export interface AuditLogPage {
  items:       AuditEntryDto[];
  page:        number;
  pageSize:    number;
  totalCount:  number;
}
