/**
 * CoreUI adapter — data table.
 *
 * Renders a responsive CoreUI/Bootstrap `.table`, mirroring the markup used by
 * list pages such as `UserListPage`. Generic over the row type for full
 * type-safety on callers' DTOs.
 */

import type { PlatformTableProps } from '@/theme/contracts';

export function CoreUiTable<TRow>({
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
    <div className="table-responsive">
      <table className={['table', 'table-hover', 'align-middle', 'mb-0', className].filter(Boolean).join(' ')}>
        <thead className="table-light">
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
