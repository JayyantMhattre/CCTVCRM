import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';

export type InquiryType = 'Contact' | 'GetQuote' | 'AmcInquiry';

export interface CreateInquiryPayload {
  readonly inquiryType: InquiryType;
  readonly name: string;
  readonly organization?: string | null;
  readonly email: string;
  readonly phone: string;
  readonly city: string;
  readonly address?: string | null;
  readonly requirementSummary?: string | null;
  readonly preferredPlanCode?: string | null;
  readonly sourcePage: string;
}

export const inquiriesApi = {
  submit: async (payload: CreateInquiryPayload): Promise<void> => {
    await apiClient.post(ENDPOINTS.cctv.inquiries, {
      inquiryType: payload.inquiryType,
      name: payload.name,
      organization: payload.organization ?? null,
      email: payload.email,
      phone: payload.phone,
      city: payload.city,
      address: payload.address ?? null,
      requirementSummary: payload.requirementSummary ?? null,
      preferredPlanCode: payload.preferredPlanCode ?? null,
      sourcePage: payload.sourcePage,
    });
  },
};
