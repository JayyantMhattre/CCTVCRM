/**
 * App — root component.
 *
 * Composes all providers and renders the router.
 * Keep this file minimal — all configuration lives in the individual
 * provider and router files.
 */

import { AppProviders } from '@/core/providers/AppProviders';
import { AppRouter }    from '@/core/router';

export function App() {
  return (
    <AppProviders>
      <AppRouter />
    </AppProviders>
  );
}
