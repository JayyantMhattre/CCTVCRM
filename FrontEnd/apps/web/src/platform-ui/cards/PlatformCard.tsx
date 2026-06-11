/**
 * PlatformCard — theme-agnostic content card.
 *
 * Delegates to the active theme adapter's `Card`.
 */

import { useTheme } from '@/theme';
import type { PlatformCardProps } from '@/theme';

export function PlatformCard(props: PlatformCardProps) {
  const { adapter } = useTheme();
  const Card = adapter.card.Card;
  return <Card {...props} />;
}
