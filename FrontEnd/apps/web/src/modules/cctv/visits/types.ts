export type VisitReportStatus = 'Draft' | 'Submitted' | 'Returned' | 'Approved';

export interface VisitAttachment {
  readonly id: string;
  readonly fileId: string;
  readonly attachmentType: string;
  readonly title: string | null;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface VisitSummary {
  readonly id: string;
  readonly serviceScheduleId: string;
  readonly scheduleNumber: string;
  readonly siteId: string;
  readonly engineerId: string;
  readonly reportStatus: VisitReportStatus;
  readonly startedAtUtc: string | null;
  readonly completedAtUtc: string | null;
  readonly isCustomerVisible: boolean;
  readonly createdAtUtc: string;
}

export interface VisitDetail extends VisitSummary {
  readonly visitRemarks: string | null;
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
  readonly hasSelfie: boolean;
  readonly hasGps: boolean;
  readonly hasBeforeDuringAfterPhoto: boolean;
  readonly hasSignature: boolean;
  readonly hasMinimumRemarks: boolean;
  readonly attachments: readonly VisitAttachment[];
}

export interface VisitListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly reportStatus?: VisitReportStatus;
}

export interface PagedVisits {
  readonly items: readonly VisitSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}
