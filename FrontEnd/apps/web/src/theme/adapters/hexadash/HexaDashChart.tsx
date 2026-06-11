/**
 * HexaDash adapter — chart frame.
 *
 * ABSTRACTION ONLY — no charting library is bound (no ApexCharts, Chart.js,
 * Recharts, Google Charts, or Ant Design). Renders the HexaDash card-style frame
 * and either the caller-supplied chart element (children) or a neutral
 * placeholder. A concrete backend is a later decision.
 */

import type { PlatformChartProps } from '@/theme/contracts';

export function HexaDashChart({
  title,
  ariaLabel,
  options,
  children,
  className,
}: PlatformChartProps) {
  const height = options?.height ?? 320;
  const minHeight = typeof height === 'number' ? `${height}px` : height;
  const label = ariaLabel ?? (typeof title === 'string' ? title : undefined);

  return (
    <figure className={['hexadash-chart', className].filter(Boolean).join(' ')}>
      {title ? <figcaption className="hexadash-chart-title">{title}</figcaption> : null}

      <div className="hexadash-chart-body" role="img" aria-label={label}>
        {children ?? (
          <div className="hexadash-chart-placeholder" style={{ minHeight }}>
            No chart backend configured.
          </div>
        )}
      </div>
    </figure>
  );
}
