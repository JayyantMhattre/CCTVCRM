/** Webhook module DTOs aligned with backend contracts. */

export interface WebhookSubscription {
  id: string;
  tenantId: string;
  name: string;
  endpointUrl: string;
  enabled: boolean;
  createdBy: string;
  createdOnUtc: string;
  updatedOnUtc: string;
}

export interface WebhookSubscriptionSecret extends WebhookSubscription {
  secret: string;
}

export type WebhookDeliveryStatus =
  | 'Pending'
  | 'Succeeded'
  | 'Failed'
  | 'Retrying'
  | 'DeadLettered';

export interface WebhookDelivery {
  id: string;
  subscriptionId: string;
  tenantId: string;
  eventName: string;
  eventVersion: string;
  correlationId: string | null;
  attemptNumber: number;
  retryCount: number;
  status: WebhookDeliveryStatus;
  responseCode: number | null;
  responseBody: string | null;
  lastFailureReason: string | null;
  lastFailureCode: number | null;
  nextRetryOnUtc: string | null;
  startedOnUtc: string;
  completedOnUtc: string | null;
}

export interface WebhookDeadLetter {
  id: string;
  deliveryId: string;
  subscriptionId: string;
  tenantId: string;
  eventName: string;
  payload: string;
  failureReason: string | null;
  failureCode: number | null;
  retryCount: number;
  correlationId: string | null;
  createdOnUtc: string;
}

export interface DeliveryFilters {
  subscriptionId?: string;
  eventName?: string;
  status?: WebhookDeliveryStatus | '';
  fromUtc?: string;
  toUtc?: string;
  limit?: number;
}

export interface DeadLetterFilters {
  subscriptionId?: string;
  eventName?: string;
  fromUtc?: string;
  toUtc?: string;
  limit?: number;
}

export interface CreateSubscriptionRequest {
  name: string;
  endpointUrl: string;
  subscribedEventNames?: string[];
}

export interface UpdateSubscriptionRequest {
  name: string;
  endpointUrl: string;
  enabled: boolean;
  subscribedEventNames?: string[];
}

export interface WebhookHealthMetrics {
  totalDeliveries: number;
  successCount: number;
  failureCount: number;
  retryingCount: number;
  retryCount: number;
  deadLetterCount: number;
  successRate: number;
  failureRate: number;
  averageDeliveryMs: number;
}

export const WEBHOOK_PERMISSIONS = {
  read: 'webhooks:read',
  manage: 'webhooks:manage',
} as const;
