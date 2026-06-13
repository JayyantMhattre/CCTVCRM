export type PlanStatus = 'Active' | 'Retired';
export type PlanVersionStatus = 'Draft' | 'Published' | 'Superseded';

export interface AmcPlanSummary {
  readonly id: string;
  readonly planCode: string;
  readonly name: string;
  readonly description: string | null;
  readonly status: PlanStatus;
  readonly publishedVersionCount: number;
  readonly createdAtUtc: string;
}

export interface AmcPlanVersionSummary {
  readonly id: string;
  readonly versionNo: number;
  readonly price: number;
  readonly visitFrequencyPerYear: number;
  readonly effectiveFrom: string;
  readonly status: PlanVersionStatus;
  readonly createdAtUtc: string;
}

export interface AmcPlanDetail extends AmcPlanSummary {
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
  readonly versions: readonly AmcPlanVersionSummary[];
}

export interface AmcPlanVersionDetail {
  readonly id: string;
  readonly planId: string;
  readonly planCode: string;
  readonly planName: string;
  readonly versionNo: number;
  readonly price: number;
  readonly visitFrequencyPerYear: number;
  readonly includedServices: readonly string[];
  readonly slaTerms: string;
  readonly effectiveFrom: string;
  readonly status: PlanVersionStatus;
  readonly isReferenced: boolean;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface AmcPlanListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: PlanStatus;
  readonly search?: string;
}

export interface PagedAmcPlans {
  readonly items: readonly AmcPlanSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}

export interface CreateAmcPlanRequest {
  readonly code: string;
  readonly name: string;
  readonly description?: string | null;
}

export interface UpdateAmcPlanRequest {
  readonly name: string;
  readonly description?: string | null;
  readonly rowVersion: number;
}

export interface CreateAmcPlanVersionRequest {
  readonly price: number;
  readonly visitFrequency: number;
  readonly includedServices: readonly string[];
  readonly slaDescription: string;
  readonly effectiveFrom: string;
}
