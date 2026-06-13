export type ContractStatus = 'Active' | 'Expired' | 'Cancelled';
export type TermStatus = 'Draft' | 'Active' | 'Expired' | 'Cancelled';
export type TermOrigin = 'New' | 'Renewal';

export interface AmcContractSummary {
  readonly id: string;
  readonly contractNumber: string;
  readonly siteId: string;
  readonly customerId: string;
  readonly status: ContractStatus;
  readonly activeTermId: string | null;
  readonly activeTermEndDate: string | null;
  readonly planCode: string | null;
  readonly createdAtUtc: string;
}

export interface AmcContractTermSummary {
  readonly id: string;
  readonly termNo: number;
  readonly planVersionId: string;
  readonly planCode: string;
  readonly planVersionNo: number;
  readonly startDate: string;
  readonly endDate: string;
  readonly agreedPrice: number;
  readonly status: TermStatus;
  readonly origin: TermOrigin;
  readonly renewalRequestedByCustomer: boolean;
  readonly renewalRequestedAtUtc: string | null;
  readonly rowVersion: number;
}

export interface AmcContractDetail {
  readonly id: string;
  readonly contractNumber: string;
  readonly siteId: string;
  readonly customerId: string;
  readonly sourceLeadId: string | null;
  readonly status: ContractStatus;
  readonly createdAtUtc: string;
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
  readonly terms: readonly AmcContractTermSummary[];
}

export interface AmcContractListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: ContractStatus;
  readonly siteId?: string;
  readonly customerId?: string;
  readonly search?: string;
}

export interface PagedAmcContracts {
  readonly items: readonly AmcContractSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}
