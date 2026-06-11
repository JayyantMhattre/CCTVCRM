import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  CreateSubscriptionRequest,
  DeadLetterFilters,
  DeliveryFilters,
  UpdateSubscriptionRequest,
  WebhookDeadLetter,
  WebhookDelivery,
  WebhookSubscription,
  WebhookSubscriptionSecret,
} from './types';

function mapSubscription(raw: Record<string, unknown>): WebhookSubscription {
  return {
    id: String(raw.id ?? raw.Id),
    tenantId: String(raw.tenantId ?? raw.TenantId),
    name: String(raw.name ?? raw.Name),
    endpointUrl: String(raw.endpointUrl ?? raw.EndpointUrl),
    enabled: Boolean(raw.enabled ?? raw.Enabled),
    createdBy: String(raw.createdBy ?? raw.CreatedBy),
    createdOnUtc: String(raw.createdOnUtc ?? raw.CreatedOnUtc),
    updatedOnUtc: String(raw.updatedOnUtc ?? raw.UpdatedOnUtc),
  };
}

function mapDelivery(raw: Record<string, unknown>): WebhookDelivery {
  return {
    id: String(raw.id ?? raw.Id),
    subscriptionId: String(raw.subscriptionId ?? raw.SubscriptionId),
    tenantId: String(raw.tenantId ?? raw.TenantId),
    eventName: String(raw.eventName ?? raw.EventName),
    eventVersion: String(raw.eventVersion ?? raw.EventVersion),
    correlationId: (raw.correlationId ?? raw.CorrelationId) as string | null,
    attemptNumber: Number(raw.attemptNumber ?? raw.AttemptNumber ?? 1),
    retryCount: Number(raw.retryCount ?? raw.RetryCount ?? 0),
    status: String(raw.status ?? raw.Status) as WebhookDelivery['status'],
    responseCode: (raw.responseCode ?? raw.ResponseCode) as number | null,
    responseBody: (raw.responseBody ?? raw.ResponseBody) as string | null,
    lastFailureReason: (raw.lastFailureReason ?? raw.LastFailureReason) as string | null,
    lastFailureCode: (raw.lastFailureCode ?? raw.LastFailureCode) as number | null,
    nextRetryOnUtc: (raw.nextRetryOnUtc ?? raw.NextRetryOnUtc) as string | null,
    startedOnUtc: String(raw.startedOnUtc ?? raw.StartedOnUtc),
    completedOnUtc: (raw.completedOnUtc ?? raw.CompletedOnUtc) as string | null,
  };
}

function mapDeadLetter(raw: Record<string, unknown>): WebhookDeadLetter {
  return {
    id: String(raw.id ?? raw.Id),
    deliveryId: String(raw.deliveryId ?? raw.DeliveryId),
    subscriptionId: String(raw.subscriptionId ?? raw.SubscriptionId),
    tenantId: String(raw.tenantId ?? raw.TenantId),
    eventName: String(raw.eventName ?? raw.EventName),
    payload: String(raw.payload ?? raw.Payload),
    failureReason: (raw.failureReason ?? raw.FailureReason) as string | null,
    failureCode: (raw.failureCode ?? raw.FailureCode) as number | null,
    retryCount: Number(raw.retryCount ?? raw.RetryCount ?? 0),
    correlationId: (raw.correlationId ?? raw.CorrelationId) as string | null,
    createdOnUtc: String(raw.createdOnUtc ?? raw.CreatedOnUtc),
  };
}

function mapSecret(raw: Record<string, unknown>): WebhookSubscriptionSecret {
  return {
    ...mapSubscription(raw),
    secret: String(raw.secret ?? raw.Secret),
  };
}

export const webhooksApi = {
  listSubscriptions: async (): Promise<WebhookSubscription[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.webhooks.subscriptions);
    return (res.data ?? []).map((item) => mapSubscription(item as Record<string, unknown>));
  },

  getSubscription: async (id: string): Promise<WebhookSubscription> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.webhooks.subscription(id));
    return mapSubscription(res.data);
  },

  createSubscription: async (body: CreateSubscriptionRequest): Promise<WebhookSubscriptionSecret> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.webhooks.subscriptions, body);
    return mapSecret(res.data);
  },

  updateSubscription: async (id: string, body: UpdateSubscriptionRequest): Promise<WebhookSubscription> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.webhooks.subscription(id), body);
    return mapSubscription(res.data);
  },

  rotateSecret: async (id: string): Promise<WebhookSubscriptionSecret> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.webhooks.rotateSecret(id));
    return mapSecret(res.data);
  },

  disableSubscription: async (id: string): Promise<WebhookSubscription> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.webhooks.disable(id));
    return mapSubscription(res.data);
  },

  listDeliveries: async (filters?: DeliveryFilters): Promise<WebhookDelivery[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.webhooks.deliveries, {
      params: {
        subscriptionId: filters?.subscriptionId || undefined,
        eventName: filters?.eventName || undefined,
        status: filters?.status || undefined,
        fromUtc: filters?.fromUtc,
        toUtc: filters?.toUtc,
        limit: filters?.limit ?? 100,
      },
    });
    return (res.data ?? []).map((item) => mapDelivery(item as Record<string, unknown>));
  },

  getDelivery: async (id: string): Promise<WebhookDelivery> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.webhooks.delivery(id));
    return mapDelivery(res.data);
  },

  retryDelivery: async (id: string): Promise<WebhookDelivery> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.webhooks.retryDelivery(id));
    return mapDelivery(res.data);
  },

  listDeadLetters: async (filters?: DeadLetterFilters): Promise<WebhookDeadLetter[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.webhooks.deadLetters, {
      params: {
        subscriptionId: filters?.subscriptionId || undefined,
        eventName: filters?.eventName || undefined,
        fromUtc: filters?.fromUtc,
        toUtc: filters?.toUtc,
        limit: filters?.limit ?? 100,
      },
    });
    return (res.data ?? []).map((item) => mapDeadLetter(item as Record<string, unknown>));
  },

  getDeadLetter: async (id: string): Promise<WebhookDeadLetter> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.webhooks.deadLetter(id));
    return mapDeadLetter(res.data);
  },

  replayDeadLetter: async (id: string): Promise<WebhookDelivery> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.webhooks.replayDeadLetter(id));
    return mapDelivery(res.data);
  },
};
