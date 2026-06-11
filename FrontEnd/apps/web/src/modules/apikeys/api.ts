import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type {
  ApiKey,
  ApiKeyCreated,
  CreateApiKeyRequest,
  UpdateApiKeyScopesRequest,
} from './types';

function mapApiKey(raw: Record<string, unknown>): ApiKey {
  const scopes = (raw.scopes ?? raw.Scopes) as unknown;
  return {
    id: String(raw.id ?? raw.Id),
    tenantId: String(raw.tenantId ?? raw.TenantId),
    name: String(raw.name ?? raw.Name),
    description: String(raw.description ?? raw.Description ?? ''),
    keyPrefix: String(raw.keyPrefix ?? raw.KeyPrefix),
    environment: String(raw.environment ?? raw.Environment ?? 'prod'),
    scopes: Array.isArray(scopes) ? scopes.map(String) : [],
    createdBy: String(raw.createdBy ?? raw.CreatedBy),
    createdOnUtc: String(raw.createdOnUtc ?? raw.CreatedOnUtc),
    expiresOnUtc: (raw.expiresOnUtc ?? raw.ExpiresOnUtc) as string | null,
    lastUsedOnUtc: (raw.lastUsedOnUtc ?? raw.LastUsedOnUtc) as string | null,
    revokedOnUtc: (raw.revokedOnUtc ?? raw.RevokedOnUtc) as string | null,
    enabled: Boolean(raw.enabled ?? raw.Enabled),
    requestCount: Number(raw.requestCount ?? raw.RequestCount ?? 0),
    successCount: Number(raw.successCount ?? raw.SuccessCount ?? 0),
    failureCount: Number(raw.failureCount ?? raw.FailureCount ?? 0),
    lastCorrelationId: (raw.lastCorrelationId ?? raw.LastCorrelationId) as string | null,
  };
}

function mapApiKeyCreated(raw: Record<string, unknown>): ApiKeyCreated {
  const scopes = (raw.scopes ?? raw.Scopes) as unknown;
  return {
    id: String(raw.id ?? raw.Id),
    tenantId: String(raw.tenantId ?? raw.TenantId),
    name: String(raw.name ?? raw.Name),
    keyPrefix: String(raw.keyPrefix ?? raw.KeyPrefix),
    plaintextKey: String(raw.plaintextKey ?? raw.PlaintextKey),
    scopes: Array.isArray(scopes) ? scopes.map(String) : [],
    createdOnUtc: String(raw.createdOnUtc ?? raw.CreatedOnUtc),
    expiresOnUtc: (raw.expiresOnUtc ?? raw.ExpiresOnUtc) as string | null,
  };
}

export const apikeysApi = {
  list: async (): Promise<ApiKey[]> => {
    const res = await apiClient.get<unknown[]>(ENDPOINTS.apikeys.list);
    return (res.data ?? []).map((item) => mapApiKey(item as Record<string, unknown>));
  },

  get: async (id: string): Promise<ApiKey> => {
    const res = await apiClient.get<Record<string, unknown>>(ENDPOINTS.apikeys.byId(id));
    return mapApiKey(res.data);
  },

  create: async (body: CreateApiKeyRequest): Promise<ApiKeyCreated> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.apikeys.list, body);
    return mapApiKeyCreated(res.data);
  },

  rotate: async (id: string): Promise<ApiKeyCreated> => {
    const res = await apiClient.post<Record<string, unknown>>(ENDPOINTS.apikeys.rotate(id));
    return mapApiKeyCreated(res.data);
  },

  revoke: async (id: string): Promise<void> => {
    await apiClient.post(ENDPOINTS.apikeys.revoke(id));
  },

  updateScopes: async (id: string, body: UpdateApiKeyScopesRequest): Promise<ApiKey> => {
    const res = await apiClient.put<Record<string, unknown>>(ENDPOINTS.apikeys.scopes(id), body);
    return mapApiKey(res.data);
  },
};
