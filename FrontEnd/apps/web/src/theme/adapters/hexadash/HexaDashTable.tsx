/**
 * HexaDash adapter — data table.
 *
 * Renders a HexaDash-styled table (token-driven `.hexadash-table`). Generic over
 * the row type for full type-safety. No Ant Design `Table` / Redux `DataTable`.
 */

import type { PlatformTableProps } from '@/theme/contracts';

export function HexaDashTable<TRow>({
  columns,
  rows,
  rowKey,
  empty,
  className,
}: PlatformTableProps<TRow>) {
  if (rows.length === 0 && empty !== undefined) {
    return <>{empty}</>;
  }

  return (
    <div className="hexadash-table-wrap">
      <table className={['hexadash-table', className].filter(Boolean).join(' ')}>
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.key} className={column.className}>
                {column.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={rowKey(row)}>
              {columns.map((column) => (
                <td key={column.key} className={column.className}>
                  {column.render(row)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
