/**
 * ChartContract — the chart surface a theme must implement.
 *
 * IMPORTANT: this is an ABSTRACTION ONLY. The platform ships NO charting library
 * by default and this contract does not bind to ApexCharts, Chart.js, Recharts,
 * Ant Design, or any vendor. It defines a neutral data model (series, options)
 * and a themed frame. A concrete charting backend is a deliberate, later
 * decision (see the compatibility report): when chosen, the adapter renders it
 * inside the frame; until then the adapter renders the caller-supplied child or
 * a neutral placeholder.
 */

import type { ComponentType, ReactNode } from 'react';

/** Supported chart shapes (neutral; not tied to any library). */
export type ChartType =
  | 'line'
  | 'bar'
  | 'area'
  | 'pie'
  | 'donut'
  | 'scatter'
  | 'radar';

/** A single data series. */
export interface ChartSeries {
  /** Series display name (legend). */
  name: string;
  /** Numeric values aligned to {@link ChartData.categories}. */
  data: readonly number[];
  /** Optional explicit colour; otherwise the theme palette is used. */
  color?: string;
}

/** The data fed to a chart: shared category axis + one or more series. */
export interface ChartData {
  /** Category labels for the x-axis (or slice labels for pie/donut). */
  categories: readonly (string | number)[];
  /** One or more data series. */
  series: readonly ChartSeries[];
}

/** Presentation options (neutral; the theme maps these to its backend). */
export interface ChartOptions {
  /** Chart height in px or any CSS length. Defaults to a theme value. */
  height?: number | string;
  /** Show the legend. */
  showLegend?: boolean;
  /** Show axis grid lines. */
  showGrid?: boolean;
  /** Stack series (for bar/area). */
  stacked?: boolean;
  /** Colour palette override (theme provides defaults). */
  colors?: readonly string[];
}

/** Props for a themed chart. */
export interface PlatformChartProps {
  /** Chart shape. */
  type: ChartType;
  /** Series + categories. */
  data: ChartData;
  /** Optional presentation options. */
  options?: ChartOptions;
  /** Optional frame title. */
  title?: ReactNode;
  /** Accessible description of what the chart conveys. */
  ariaLabel?: string;
  /** Extra classes for the wrapper. */
  className?: string;
  /**
   * Optional concrete chart element supplied by the caller. Since the platform
   * ships no charting library, the adapter renders this child inside the themed
   * frame when present, or a neutral placeholder when absent.
   */
  children?: ReactNode;
}

/** A theme's chart component. */
export type PlatformChartComponent = ComponentType<PlatformChartProps>;

/** The chart portion of a {@link ThemeAdapter}. */
export interface ChartContract {
  Chart: PlatformChartComponent;
}
