import { format, formatDistanceToNow } from 'date-fns';

export function formatUtc(iso: string | null | undefined): string {
  if (!iso) return '—';
  try {
    return format(new Date(iso), 'yyyy-MM-dd HH:mm:ss');
  } catch {
    return iso;
  }
}

export function formatRelative(iso: string | null | undefined): string {
  if (!iso) return '—';
  try {
    return formatDistanceToNow(new Date(iso), { addSuffix: true });
  } catch {
    return iso;
  }
}

export function deliveryDurationMs(
  startedOnUtc: string,
  completedOnUtc: string | null,
): number | null {
  if (!completedOnUtc) return null;
  const ms = new Date(completedOnUtc).getTime() - new Date(startedOnUtc).getTime();
  return ms >= 0 ? ms : null;
}

export function formatDuration(ms: number | null): string {
  if (ms === null) return '—';
  if (ms < 1000) return `${ms} ms`;
  return `${(ms / 1000).toFixed(2)} s`;
}

export function formatPercent(value: number): string {
  return `${(value * 100).toFixed(1)}%`;
}

export function tryFormatJson(value: string | null | undefined): string {
  if (!value) return '';
  try {
    return JSON.stringify(JSON.parse(value), null, 2);
  } catch {
    return value;
  }
}
