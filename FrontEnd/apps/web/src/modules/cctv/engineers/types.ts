export type EngineerStatus = 'Active' | 'Inactive';

export interface EngineerSummary {
  readonly id: string;
  readonly engineerNumber: string;
  readonly name: string;
  readonly phone: string;
  readonly status: EngineerStatus;
  readonly platformUserId: string | null;
  readonly createdAtUtc: string;
}

export interface EngineerDetail extends EngineerSummary {
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
}

export interface PagedEngineers {
  readonly items: readonly EngineerSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}

export interface EngineerListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: EngineerStatus;
  readonly search?: string;
}

export interface CreateEngineerRequest {
  readonly name: string;
  readonly phone: string;
  readonly platformUserId?: string | null;
}

export interface UpdateEngineerRequest {
  readonly name: string;
  readonly phone: string;
  readonly platformUserId?: string | null;
  readonly rowVersion: number;
}

export interface ChangeEngineerStatusRequest {
  readonly status: EngineerStatus;
  readonly rowVersion: number;
}

export interface EngineerWorkload {
  readonly engineerId: string;
  readonly activeScheduleCount: number;
  readonly openTicketCount: number;
}
