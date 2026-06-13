export type TicketStatus = 'Open' | 'Assigned' | 'InProgress' | 'Resolved' | 'Closed' | 'Reopened';
export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type TicketSource = 'Customer' | 'Admin' | 'EngineerVisit';

export interface TicketSummary {
  readonly id: string;
  readonly ticketNumber: string;
  readonly customerId: string;
  readonly siteId: string;
  readonly subject: string;
  readonly priority: TicketPriority;
  readonly status: TicketStatus;
  readonly assignedEngineerId: string | null;
  readonly createdAtUtc: string;
  readonly resolvedAtUtc: string | null;
  readonly closedAtUtc: string | null;
  readonly reopenCount: number;
}

export interface TicketComment {
  readonly id: string;
  readonly comment: string;
  readonly authorRole: string;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface TicketAttachment {
  readonly id: string;
  readonly fileId: string;
  readonly title: string | null;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface TicketStatusHistory {
  readonly id: string;
  readonly fromStatus: string | null;
  readonly toStatus: string;
  readonly reason: string | null;
  readonly changedAtUtc: string;
  readonly changedBy: string;
}

export interface TicketDetail extends TicketSummary {
  readonly amcContractId: string | null;
  readonly originServiceVisitId: string | null;
  readonly source: TicketSource;
  readonly description: string;
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
  readonly comments: readonly TicketComment[];
  readonly attachments: readonly TicketAttachment[];
  readonly statusHistory: readonly TicketStatusHistory[];
}

export interface TicketListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: TicketStatus;
  readonly priority?: TicketPriority;
  readonly customerId?: string;
  readonly siteId?: string;
  readonly engineerId?: string;
  readonly search?: string;
}

export interface PagedTickets {
  readonly items: readonly TicketSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}
