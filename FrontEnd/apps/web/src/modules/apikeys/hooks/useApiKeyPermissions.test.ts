import { describe, expect, it, vi, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import { useApiKeyPermissions } from './useApiKeyPermissions';

vi.mock('@/shared/hooks/usePermission', () => ({
  usePermission: vi.fn(),
}));

import { usePermission } from '@/shared/hooks/usePermission';

describe('useApiKeyPermissions', () => {
  beforeEach(() => {
    vi.mocked(usePermission).mockReturnValue({
      hasPermission: (p: string) => p === 'apikeys:read',
      hasAllPermissions: () => false,
      hasAnyPermission: () => true,
    });
  });

  it('grants read when apikeys:read present', () => {
    const { result } = renderHook(() => useApiKeyPermissions());
    expect(result.current.canRead).toBe(true);
    expect(result.current.canManage).toBe(false);
  });

  it('grants manage when apikeys:manage present', () => {
    vi.mocked(usePermission).mockReturnValue({
      hasPermission: (p: string) => p === 'apikeys:manage',
      hasAllPermissions: () => false,
      hasAnyPermission: () => true,
    });
    const { result } = renderHook(() => useApiKeyPermissions());
    expect(result.current.canRead).toBe(true);
    expect(result.current.canManage).toBe(true);
  });
});
