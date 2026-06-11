/**
 * PlatformTable — theme-agnostic, generic data table.
 *
 * Delegates rendering to the active theme adapter's `Table`. Generic over the
 * row type so callers keep full type-safety. The adapter component is invoked
 * directly (it is a generic call signature) to preserve the row-type generic.
 */

import { useTheme } from '@/theme';
import type { PlatformTableProps } from '@/theme';

export function PlatformTable<TRow>(props: PlatformTableProps<TRow>) {
  const { adapter } = useTheme();
  return adapter.table.Table(props);
}
