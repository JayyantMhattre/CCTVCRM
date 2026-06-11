/**
 * Chart types — the platform's neutral chart data model.
 *
 * Re-exported from the theme `ChartContract` so there is a single source of
 * truth. These types are deliberately library-agnostic: the platform ships no
 * charting backend (no ApexCharts, Chart.js, Recharts, or Ant Design). A
 * concrete backend is added later, behind the adapter, without changing this
 * model.
 */

export type {
  ChartType,
  ChartSeries,
  ChartData,
  ChartOptions,
  PlatformChartProps,
} from '@/theme';
