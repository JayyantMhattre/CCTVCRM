export type LeadStatus =
  | 'New'
  | 'Contacted'
  | 'Qualified'
  | 'QuotationSent'
  | 'Negotiation'
  | 'Won'
  | 'Lost'
  | 'Converted';

export type LeadSource = 'WebsiteContact' | 'AmcInquiry' | 'GetQuote' | 'Manual';

export interface LeadSummary {
  readonly id: string;
  readonly leadNumber: string;
  readonly status: LeadStatus;
  readonly source: LeadSource;
  readonly contactName: string;
  readonly organizationName: string | null;
  readonly email: string;
  readonly phone: string;
  readonly city: string;
  readonly ownerUserId: string | null;
  readonly createdAtUtc: string;
}

export interface LeadDetail extends LeadSummary {
  readonly address: string | null;
  readonly requirementSummary: string | null;
  readonly convertedCustomerId: string | null;
  readonly convertedSiteId: string | null;
  readonly convertedContractId: string | null;
  readonly convertedAtUtc: string | null;
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
  readonly activityCount: number;
  readonly remarkCount: number;
  readonly attachmentCount: number;
}

export interface LeadActivity {
  readonly id: string;
  readonly activityType: string;
  readonly fromStatus: LeadStatus | null;
  readonly toStatus: LeadStatus | null;
  readonly description: string;
  readonly occurredAtUtc: string;
  readonly createdBy: string;
}

export interface LeadRemark {
  readonly id: string;
  readonly text: string;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface PagedLeads {
  readonly items: readonly LeadSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}

export interface LeadListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: LeadStatus;
  readonly search?: string;
}

export interface ChangeLeadStatusRequest {
  readonly toStatus: LeadStatus;
  readonly notes?: string;
  readonly rowVersion: number;
}

export interface CreateLeadRemarkRequest {
  readonly text: string;
}
