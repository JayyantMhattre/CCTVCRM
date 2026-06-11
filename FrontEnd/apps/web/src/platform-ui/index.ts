/**
 * platform-ui — the platform's stable, theme-agnostic UI API.
 *
 * Business modules import UI primitives from here (and only here). Each
 * primitive renders against a theme contract, so swapping themes never touches
 * module code.
 *
 * Import rules (enforced by convention; lint rule planned):
 *   - modules → platform-ui            ✓
 *   - modules → theme/* or @coreui/*   ✗ (use platform-ui)
 *   - platform-ui → theme              ✓
 */

export * from './layout';
export * from './navigation';
export * from './tables';
export * from './forms';
export * from './dialogs';
export * from './cards';
export * from './charts';
export * from './notifications';
export * from './badges';
export * from './avatar';
export * from './tabs';
export * from './breadcrumbs';
