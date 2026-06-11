/**
 * PlatformNavigationProvider — resolves the navigation model once and shares it.
 *
 * Responsibilities:
 *   - resolve the menu (`usePlatformNav`)
 *   - apply role filtering, permission filtering and feature filtering
 *     (all delegated to the resolver, the single source of truth)
 *   - expose the final, resolved navigation model via context
 *
 * Consumers read the model with `useNavigationModel()`. This keeps navigation
 * resolution in exactly one place and lets multiple surfaces (the shell sidebar,
 * a future top-bar, breadcrumbs, command palette, …) share it.
 */

import { createContext, useContext, type ReactNode } from 'react';
import { usePlatformNav } from './usePlatformNav';
import type { NavigationGroup } from './models';

const NavigationModelContext = createContext<readonly NavigationGroup[] | null>(null);

interface PlatformNavigationProviderProps {
  children: ReactNode;
}

export function PlatformNavigationProvider({ children }: PlatformNavigationProviderProps) {
  const model = usePlatformNav();

  return (
    <NavigationModelContext.Provider value={model}>
      {children}
    </NavigationModelContext.Provider>
  );
}

/** Read the resolved navigation model. Must be used inside the provider. */
export function useNavigationModel(): readonly NavigationGroup[] {
  const model = useContext(NavigationModelContext);
  if (model === null) {
    throw new Error('useNavigationModel must be used within a PlatformNavigationProvider.');
  }
  return model;
}
