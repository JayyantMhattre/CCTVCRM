/**
 * PlatformDialog — theme-agnostic dialog.
 *
 * Delegates to the active theme adapter's `Dialog`.
 */

import { useTheme } from '@/theme';
import type { PlatformDialogProps } from '@/theme';

export function PlatformDialog(props: PlatformDialogProps) {
  const { adapter } = useTheme();
  const Dialog = adapter.dialog.Dialog;
  return <Dialog {...props} />;
}
