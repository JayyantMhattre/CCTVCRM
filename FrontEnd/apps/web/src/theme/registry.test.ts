import { describe, expect, it } from 'vitest';
import { getThemeAdapter, getRegisteredThemeIds } from './registry';
import { DEFAULT_THEME_ID, KNOWN_THEME_IDS, resolveThemeId } from './config';
import type { ThemeAdapter } from './contracts';

/**
 * The complete contract surface every theme adapter must implement. Each entry
 * lists the component(s) for that contract. This is the single check that
 * guarantees "no missing implementations" across themes.
 */
const CONTRACT_SURFACES = {
  layout: ['Layout', 'AuthLayout'],
  navigation: ['Nav'],
  card: ['Card'],
  dialog: ['Dialog'],
  table: ['Table'],
  notification: ['Viewport'],
  badge: ['Badge'],
  avatar: ['Avatar'],
  tabs: ['Tabs'],
  breadcrumb: ['Breadcrumb'],
  chart: ['Chart'],
} as const;

function surfaceEntries(adapter: ThemeAdapter): Array<[string, string]> {
  const record = adapter as unknown as Record<string, Record<string, unknown>>;
  const entries: Array<[string, string]> = [];

  for (const [group, components] of Object.entries(CONTRACT_SURFACES)) {
    for (const component of components) {
      entries.push([group, component]);
      // Every named surface must resolve to a component (function).
      expect(typeof record[group]?.[component]).toBe('function');
    }
  }

  return entries;
}

describe('theme registry', () => {
  it('keeps CoreUI as the production default', () => {
    expect(DEFAULT_THEME_ID).toBe('coreui');
    expect(resolveThemeId()).toBe('coreui');
  });

  it('knows both coreui and hexadash', () => {
    expect(KNOWN_THEME_IDS).toContain('coreui');
    expect(KNOWN_THEME_IDS).toContain('hexadash');
    expect(getRegisteredThemeIds()).toEqual(expect.arrayContaining(['coreui', 'hexadash']));
  });

  it('resolves the hexadash adapter', () => {
    const adapter = getThemeAdapter('hexadash');
    expect(adapter.id).toBe('hexadash');
    expect(adapter.label).toBe('HexaDash');
  });

  it('resolves the coreui adapter', () => {
    const adapter = getThemeAdapter('coreui');
    expect(adapter.id).toBe('coreui');
  });

  it('implements all 11 contracts on the hexadash adapter', () => {
    const entries = surfaceEntries(getThemeAdapter('hexadash'));
    // 11 contract groups → 12 component surfaces (layout has two).
    expect(entries).toHaveLength(12);
  });

  it('implements all 11 contracts on the coreui adapter (parity)', () => {
    const entries = surfaceEntries(getThemeAdapter('coreui'));
    expect(entries).toHaveLength(12);
  });

  it('exposes the same contract surface on both adapters', () => {
    const coreui = surfaceEntries(getThemeAdapter('coreui'));
    const hexadash = surfaceEntries(getThemeAdapter('hexadash'));
    expect(hexadash).toEqual(coreui);
  });
});
