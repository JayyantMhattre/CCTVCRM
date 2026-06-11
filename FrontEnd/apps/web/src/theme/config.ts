/**
 * Theme system configuration.
 *
 * The active theme is selected at build/runtime via the `VITE_THEME`
 * environment variable. When unset or unrecognised, the platform falls back
 * to the default CoreUI theme so the application always renders.
 *
 * Adding a future theme:
 *   1. Extend the `ThemeId` union below.
 *   2. Add the id to `KNOWN_THEME_IDS`.
 *   3. Register its adapter in `registry.ts`.
 */

/**
 * Identifier for every theme the platform knows how to render.
 * Extend this union when a new theme adapter is added.
 */
export type ThemeId = 'coreui' | 'hexadash';

/**
 * The theme used when `VITE_THEME` is missing or invalid.
 * CoreUI remains the production default; HexaDash is opt-in via `VITE_THEME`.
 */
export const DEFAULT_THEME_ID: ThemeId = 'coreui';

/** Every theme id the registry is expected to resolve. */
export const KNOWN_THEME_IDS: readonly ThemeId[] = ['coreui', 'hexadash'];

/**
 * Resolves the active theme id from the `VITE_THEME` environment variable.
 * Falls back to {@link DEFAULT_THEME_ID} for any unknown value.
 */
export function resolveThemeId(): ThemeId {
  const raw = import.meta.env.VITE_THEME as string | undefined;

  if (raw && (KNOWN_THEME_IDS as readonly string[]).includes(raw)) {
    return raw as ThemeId;
  }

  return DEFAULT_THEME_ID;
}
