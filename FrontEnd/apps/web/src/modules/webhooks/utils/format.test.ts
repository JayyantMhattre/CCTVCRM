import { describe, expect, it } from 'vitest';
import { deliveryDurationMs, formatDuration, formatPercent, tryFormatJson } from './format';

describe('webhook format utils', () => {
  it('computes delivery duration', () => {
    const ms = deliveryDurationMs('2026-01-01T00:00:00Z', '2026-01-01T00:00:01.500Z');
    expect(ms).toBe(1500);
  });

  it('formats duration', () => {
    expect(formatDuration(250)).toBe('250 ms');
    expect(formatDuration(1500)).toBe('1.50 s');
    expect(formatDuration(null)).toBe('—');
  });

  it('formats percent', () => {
    expect(formatPercent(0.956)).toBe('95.6%');
  });

  it('pretty prints json', () => {
    expect(tryFormatJson('{"a":1}')).toContain('"a"');
  });
});
