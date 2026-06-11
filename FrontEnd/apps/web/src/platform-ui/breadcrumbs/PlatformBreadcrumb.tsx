/**
 * PlatformBreadcrumb — theme-agnostic breadcrumb trail.
 *
 * Collects its `<PlatformBreadcrumbItem>` children into a plain model and
 * delegates rendering to the active theme adapter's `Breadcrumb`. Items may
 * also be supplied directly via the `items` prop instead of children.
 */

import { Children, isValidElement } from 'react';
import type { ReactElement, ReactNode } from 'react';
import { useTheme } from '@/theme';
import type { PlatformBreadcrumbEntry } from '@/theme';
import { PlatformBreadcrumbItem } from './PlatformBreadcrumbItem';
import type { PlatformBreadcrumbItemProps } from './PlatformBreadcrumbItem';

interface PlatformBreadcrumbProps {
  /** `<PlatformBreadcrumbItem>` elements (root first, current page last). */
  children?: ReactNode;
  /** Alternative to children: supply the resolved entries directly. */
  items?: readonly PlatformBreadcrumbEntry[];
  /** Accessible label for the nav landmark. */
  ariaLabel?: string;
  /** Extra classes for the nav wrapper. */
  className?: string;
}

/** Type guard: is this child a `<PlatformBreadcrumbItem>` element? */
function isBreadcrumbItemElement(
  child: ReactNode,
): child is ReactElement<PlatformBreadcrumbItemProps> {
  return isValidElement(child) && child.type === PlatformBreadcrumbItem;
}

export function PlatformBreadcrumb({
  children,
  items,
  ariaLabel,
  className,
}: PlatformBreadcrumbProps) {
  const { adapter } = useTheme();
  const Breadcrumb = adapter.breadcrumb.Breadcrumb;

  const resolved: PlatformBreadcrumbEntry[] =
    items !== undefined
      ? [...items]
      : Children.toArray(children)
          .filter(isBreadcrumbItemElement)
          .map((element) => ({ label: element.props.label, to: element.props.to }));

  return <Breadcrumb items={resolved} ariaLabel={ariaLabel} className={className} />;
}
