/**
 * Theme registry — maps a `ThemeId` to its concrete `ThemeAdapter`.
 *
 * This is the single place where concrete themes are wired in. Adding a future
 * theme is a one-line registration here plus its adapter folder — no changes to
 * platform-ui or business modules.
 */

import type { ThemeId } from './config';
import { DEFAULT_THEME_ID } from './config';
import type { ThemeAdapter } from './contracts';
import { coreUiAdapter } from './adapters/coreui';
import { hexadashAdapter } from './adapters/hexadash';

const REGISTRY: Record<ThemeId, ThemeAdapter> = {
  coreui: coreUiAdapter,
  hexadash: hexadashAdapter,
};

/**
 * Resolves the adapter for a theme id, falling back to the default theme when
 * the id is not registered.
 */
export function getThemeAdapter(id: ThemeId): ThemeAdapter {
  return REGISTRY[id] ?? REGISTRY[DEFAULT_THEME_ID];
}

/** All registered theme ids (useful for diagnostics / theme pickers). */
export function getRegisteredThemeIds(): ThemeId[] {
  return Object.keys(REGISTRY) as ThemeId[];
}
