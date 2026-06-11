/**
 * ThemeContext — React context exposing the active theme adapter.
 *
 * Consumers should use the `useTheme()` hook rather than reading the context
 * directly. The provider lives in `ThemeProvider.tsx`.
 */

import { createContext, useContext } from 'react';
import type { ThemeId } from './config';
import type { ThemeAdapter } from './contracts';

/** Value exposed by the theme context. */
export interface ThemeContextValue {
  /** The active theme id. */
  themeId: ThemeId;
  /** The active theme adapter implementing all contracts. */
  adapter: ThemeAdapter;
}

/** Internal context — null until a `ThemeProvider` is mounted. */
export const ThemeContext = createContext<ThemeContextValue | null>(null);

/**
 * Returns the active theme context.
 * @throws if called outside a `ThemeProvider`.
 */
export function useTheme(): ThemeContextValue {
  const value = useContext(ThemeContext);

  if (value === null) {
    throw new Error('useTheme must be used within a <ThemeProvider>.');
  }

  return value;
}
