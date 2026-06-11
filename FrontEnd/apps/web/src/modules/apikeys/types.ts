/** API Keys module DTOs aligned with backend contracts. */

export interface ApiKey {
  id: string;
  tenantId: string;
  name: string;
  description: string;
  keyPrefix: string;
  environment: string;
  scopes: string[];
  createdBy: string;
  createdOnUtc: string;
  expiresOnUtc: string | null;
  lastUsedOnUtc: string | null;
  revokedOnUtc: string | null;
  enabled: boolean;
  requestCount: number;
  successCount: number;
  failureCount: number;
  lastCorrelationId: string | null;
}

export interface ApiKeyCreated extends Pick<
  ApiKey,
  'id' | 'tenantId' | 'name' | 'keyPrefix' | 'scopes' | 'createdOnUtc' | 'expiresOnUtc'
> {
  plaintextKey: string;
}

export interface CreateApiKeyRequest {
  name: string;
  description?: string;
  scopes?: string[];
  expiresOnUtc?: string | null;
}

export interface UpdateApiKeyScopesRequest {
  scopes: string[];
}

export type ApiKeyStatusFilter = '' | 'active' | 'revoked' | 'expired';

export interface ApiKeyListFilters {
  search?: string;
  status?: ApiKeyStatusFilter;
  environment?: string;
}

export const API_KEY_PERMISSIONS = {
  read: 'apikeys:read',
  manage: 'apikeys:manage',
} as const;
