export type CustomerStatus = 'Active' | 'Inactive';

export interface CustomerSummary {
  readonly id: string;
  readonly customerNumber: string;
  readonly name: string;
  readonly email: string;
  readonly phone: string;
  readonly city: string;
  readonly status: CustomerStatus;
  readonly sourceLeadId: string | null;
  readonly createdAtUtc: string;
}

export interface CustomerDetail extends CustomerSummary {
  readonly billingAddress: string;
  readonly portalUserId: string | null;
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
}

export interface PagedCustomers {
  readonly items: readonly CustomerSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}

export interface CustomerListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: CustomerStatus;
  readonly search?: string;
}

export interface CreateCustomerRequest {
  readonly name: string;
  readonly email: string;
  readonly phone: string;
  readonly billingAddress: string;
  readonly city: string;
}

export interface UpdateCustomerRequest {
  readonly name: string;
  readonly email: string;
  readonly phone: string;
  readonly billingAddress: string;
  readonly city: string;
  readonly rowVersion: number;
}

export interface ChangeCustomerStatusRequest {
  readonly status: CustomerStatus;
  readonly rowVersion: number;
}
