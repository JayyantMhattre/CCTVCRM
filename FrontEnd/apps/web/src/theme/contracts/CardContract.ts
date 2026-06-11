/**
 * CardContract — the surface used for content panels / cards.
 */

import type { ComponentType, ReactNode } from 'react';

/** Props for a themed content card. */
export interface PlatformCardProps {
  /** Optional header title. */
  title?: ReactNode;
  /** Optional right-aligned header actions. */
  actions?: ReactNode;
  /** Optional footer content. */
  footer?: ReactNode;
  /** Extra classes for the outer card element. */
  className?: string;
  /** Extra classes for the card body. */
  bodyClassName?: string;
  /** Card body content. */
  children?: ReactNode;
}

/** A theme's card component. */
export type PlatformCardComponent = ComponentType<PlatformCardProps>;

/** The card portion of a {@link ThemeAdapter}. */
export interface CardContract {
  Card: PlatformCardComponent;
}
