/**
 * TableContract — the data-table surface a theme must implement.
 *
 * The table is generic over the row type so callers keep full type-safety on
 * their DTOs. The platform supplies columns + rows; the theme renders them.
 */

import type { ReactElement, ReactNode } from 'react';

/** A single table column definition. */
export interface PlatformTableColumn<TRow> {
  /** Stable column key (used as React key). */
  key: string;
  /** Header cell content. */
  header: ReactNode;
  /** Renders the cell for a given row. */
  render: (row: TRow) => ReactNode;
  /** Optional per-column class applied to header and cells. */
  className?: string;
}

/** Props for a themed table. */
export interface PlatformTableProps<TRow> {
  /** Ordered column definitions. */
  columns: PlatformTableColumn<TRow>[];
  /** Row data. */
  rows: TRow[];
  /** Returns a stable key for a row. */
  rowKey: (row: TRow) => string;
  /** Content rendered when there are no rows. */
  empty?: ReactNode;
  /** Extra classes for the table element. */
  className?: string;
}

/**
 * A theme's table renderer — a generic component callable for any row type.
 */
export type PlatformTableComponent = <TRow>(
  props: PlatformTableProps<TRow>,
) => ReactElement | null;

/** The table portion of a {@link ThemeAdapter}. */
export interface TableContract {
  Table: PlatformTableComponent;
}
