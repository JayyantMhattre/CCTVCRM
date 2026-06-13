export type InvoiceStatus = 'Draft' | 'Generated' | 'Sent' | 'Paid' | 'Cancelled';

export type InvoiceType =
  | 'AmcRenewal'
  | 'NewAmc'
  | 'EmergencyService'
  | 'SpareReplacement'
  | 'AdditionalCharges'
  | 'Other';

export interface InvoiceSummary {
  readonly id: string;
  readonly invoiceNumber: string;
  readonly customerId: string;
  readonly siteId: string | null;
  readonly invoiceType: InvoiceType;
  readonly status: InvoiceStatus;
  readonly invoiceDate: string;
  readonly dueDate: string | null;
  readonly totalAmount: number;
  readonly createdAtUtc: string;
  readonly paidAtUtc: string | null;
}

export interface InvoiceLine {
  readonly id: string;
  readonly lineNo: number;
  readonly description: string;
  readonly quantity: number;
  readonly unitPrice: number;
  readonly lineTotal: number;
}

export interface InvoiceAttachment {
  readonly id: string;
  readonly fileId: string;
  readonly attachmentType: string;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface InvoiceStatusHistory {
  readonly id: string;
  readonly fromStatus: string | null;
  readonly toStatus: string;
  readonly changedAtUtc: string;
  readonly changedBy: string;
}

export interface InvoiceDetail extends InvoiceSummary {
  readonly amcContractTermId: string | null;
  readonly ticketId: string | null;
  readonly serviceVisitId: string | null;
  readonly subtotalAmount: number;
  readonly taxAmount: number;
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
  readonly lines: readonly InvoiceLine[];
  readonly attachments: readonly InvoiceAttachment[];
  readonly statusHistory: readonly InvoiceStatusHistory[];
}

export interface PagedInvoices {
  readonly items: readonly InvoiceSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}

export interface InvoiceListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly status?: InvoiceStatus;
  readonly invoiceType?: InvoiceType;
  readonly customerId?: string;
  readonly search?: string;
}

export interface InvoiceLineRequest {
  readonly description: string;
  readonly quantity: number;
  readonly unitPrice: number;
  readonly taxRate?: number;
  readonly sortOrder?: number;
}

export interface CreateInvoiceRequest {
  readonly customerId: string;
  readonly siteId?: string | null;
  readonly invoiceType: InvoiceType;
  readonly amcContractTermId?: string | null;
  readonly ticketId?: string | null;
  readonly serviceVisitId?: string | null;
  readonly invoiceDate: string;
  readonly dueDate?: string | null;
  readonly lines: readonly InvoiceLineRequest[];
  readonly taxAmount?: number;
}

export interface UpdateInvoiceDraftRequest {
  readonly siteId?: string | null;
  readonly invoiceType: InvoiceType;
  readonly amcContractTermId?: string | null;
  readonly ticketId?: string | null;
  readonly serviceVisitId?: string | null;
  readonly invoiceDate: string;
  readonly dueDate?: string | null;
  readonly lines: readonly InvoiceLineRequest[];
  readonly taxAmount?: number;
  readonly rowVersion: number;
}

export interface InvoicePdfInfo {
  readonly invoiceId: string;
  readonly invoiceNumber: string;
  readonly fileId: string;
  readonly downloadUrl: string;
}
