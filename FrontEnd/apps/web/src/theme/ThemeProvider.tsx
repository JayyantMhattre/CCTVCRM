/**
 * ThemeProvider — supplies the active theme adapter via React context.
 *
 * Resolves the theme from `VITE_THEME` (or an explicit `themeId` prop, useful
 * in tests / Storybook) and exposes it through `ThemeContext`. It performs no
 * rendering of its own beyond the context provider, so mounting it does not
 * change application appearance.
 */

import { useMemo } from 'react';
import type { ReactNode } from 'react';
import { ThemeContext } from './ThemeContext';
import type { ThemeContextValue } from './ThemeContext';
import { resolveThemeId } from './config';
import type { ThemeId } from './config';
import { getThemeAdapter } from './registry';

interface ThemeProviderProps {
  /** Override the resolved theme (defaults to `VITE_THEME` / the default). */
  themeId?: ThemeId;
  children: ReactNode;
}

export function ThemeProvider({ themeId, children }: ThemeProviderProps) {
  const value = useMemo<ThemeContextValue>(() => {
    const id = themeId ?? resolveThemeId();
    return { themeId: id, adapter: getThemeAdapter(id) };
  }, [themeId]);

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}
