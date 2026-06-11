import { describe, expect, it, vi, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useWebhookPermissions } from './useWebhookPermissions';

vi.mock('@/shared/hooks/usePermission', () => ({
  usePermission: vi.fn(),
}));

import { usePermission } from '@/shared/hooks/usePermission';

describe('useWebhookPermissions', () => {
  beforeEach(() => {
    vi.mocked(usePermission).mockReturnValue({
      hasPermission: (p: string) => p === 'webhooks:read',
      hasAllPermissions: () => false,
      hasAnyPermission: () => true,
    });
  });

  it('grants read when webhooks:read present', () => {
    const { result } = renderHook(() => useWebhookPermissions());
    expect(result.current.canRead).toBe(true);
    expect(result.current.canManage).toBe(false);
  });

  it('grants manage when webhooks:manage present', () => {
    vi.mocked(usePermission).mockReturnValue({
      hasPermission: (p: string) => p === 'webhooks:manage',
      hasAllPermissions: () => false,
      hasAnyPermission: () => true,
    });
    const { result } = renderHook(() => useWebhookPermissions());
    expect(result.current.canRead).toBe(true);
    expect(result.current.canManage).toBe(true);
  });
});
