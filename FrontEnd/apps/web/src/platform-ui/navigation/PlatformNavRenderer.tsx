/**
 * PlatformNavRenderer — theme-agnostic navigation renderer.
 *
 * Reads the resolved navigation model from `PlatformNavigationProvider` and hands
 * it to the active theme adapter's `Nav`, which renders it (and makes no business
 * decisions). This is the canonical way to render the platform menu with the
 * current theme anywhere in the app.
 *
 * The application shell passes the model to its theme `Layout` directly (see
 * `PlatformLayout`), but any other surface can drop in `<PlatformNavRenderer />`.
 */

import { useTheme } from '@/theme';
import { useNavigationModel } from './PlatformNavigationProvider';

interface PlatformNavRendererProps {
  /** Accessible label for the nav landmark. */
  ariaLabel?: string;
}

export function PlatformNavRenderer({ ariaLabel }: PlatformNavRendererProps) {
  const { adapter } = useTheme();
  const groups = useNavigationModel();
  const Nav = adapter.navigation.Nav;

  return <Nav groups={groups} ariaLabel={ariaLabel} />;
}
