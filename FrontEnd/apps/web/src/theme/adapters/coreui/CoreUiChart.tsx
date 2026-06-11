/**
 * CoreUI adapter — chart frame.
 *
 * ABSTRACTION ONLY — no charting library is bound here (no ApexCharts, Chart.js,
 * Recharts, or Ant Design). This renders the consistent card-style frame (title
 * + accessible region) into which a concrete chart is placed once a charting
 * backend is introduced. When the caller supplies a chart element as children it
 * is rendered inside the frame; otherwise a neutral placeholder is shown.
 */

import type { PlatformChartProps } from '@/theme/contracts';

export function CoreUiChart({
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
    <figure
      className={['card', 'border-0', 'shadow-sm', 'mb-0', className].filter(Boolean).join(' ')}
    >
      {title ? (
        <figcaption className="card-header bg-transparent fw-semibold">{title}</figcaption>
      ) : null}

      <div className="card-body" role="img" aria-label={label}>
        {children ?? (
          <div
            className="d-flex align-items-center justify-content-center text-body-secondary small"
            style={{ minHeight }}
          >
            No chart backend configured.
          </div>
        )}
      </div>
    </figure>
  );
}
