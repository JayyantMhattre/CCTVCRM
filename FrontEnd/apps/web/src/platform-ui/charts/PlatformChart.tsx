/**
 * PlatformChart — theme-agnostic chart container.
 *
 * Delegates to the active theme adapter's `Chart`. The platform intentionally
 * ships NO chart library by default (see the compatibility report: adding one
 * is a deliberate, later decision). The adapter renders the themed frame and
 * either the caller-supplied chart element (passed as children) or a neutral
 * placeholder. The neutral data model (`ChartData`, `ChartSeries`,
 * `ChartOptions`, `ChartType`) is ready for a backend without API changes.
 */

import { useTheme } from '@/theme';
import type { PlatformChartProps } from '@/theme';

export function PlatformChart(props: PlatformChartProps) {
  const { adapter } = useTheme();
  const Chart = adapter.chart.Chart;
  return <Chart {...props} />;
}
