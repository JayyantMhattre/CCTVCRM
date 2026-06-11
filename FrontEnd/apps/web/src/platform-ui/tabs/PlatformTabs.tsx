/**
 * PlatformTabs — theme-agnostic tabbed content.
 *
 * Collects its `<PlatformTab>` children into a plain model, owns the active-tab
 * state (controlled or uncontrolled), and delegates rendering to the active
 * theme adapter's `Tabs`. The theme makes no state decisions.
 */

import { Children, isValidElement, useState } from 'react';
import type { ReactElement, ReactNode } from 'react';
import { useTheme } from '@/theme';
import type { PlatformTabItem } from '@/theme';
import { PlatformTab } from './PlatformTab';
import type { PlatformTabProps } from './PlatformTab';

interface PlatformTabsProps {
  /** `<PlatformTab>` elements. */
  children: ReactNode;
  /** Initial active tab id (uncontrolled mode). Defaults to the first tab. */
  defaultActiveId?: string;
  /** Active tab id (controlled mode). When set, the parent owns the state. */
  activeId?: string;
  /** Notified when the active tab changes. */
  onChange?: (id: string) => void;
  /** Accessible label for the tablist. */
  ariaLabel?: string;
  /** Extra classes for the wrapper. */
  className?: string;
}

/** Type guard: is this child a `<PlatformTab>` element? */
function isPlatformTabElement(
  child: ReactNode,
): child is ReactElement<PlatformTabProps> {
  return isValidElement(child) && child.type === PlatformTab;
}

export function PlatformTabs({
  children,
  defaultActiveId,
  activeId: controlledActiveId,
  onChange,
  ariaLabel,
  className,
}: PlatformTabsProps) {
  const { adapter } = useTheme();
  const TabsView = adapter.tabs.Tabs;

  const items: PlatformTabItem[] = Children.toArray(children)
    .filter(isPlatformTabElement)
    .map((element) => ({
      id: element.props.tabId,
      label: element.props.label,
      content: element.props.children,
      disabled: element.props.disabled,
    }));

  const firstId = items[0]?.id ?? '';
  const [internalActiveId, setInternalActiveId] = useState(defaultActiveId ?? firstId);

  const isControlled = controlledActiveId !== undefined;
  const activeId = isControlled ? controlledActiveId : internalActiveId;

  function handleChange(id: string) {
    if (!isControlled) {
      setInternalActiveId(id);
    }
    onChange?.(id);
  }

  return (
    <TabsView
      items={items}
      activeId={activeId}
      onChange={handleChange}
      ariaLabel={ariaLabel}
      className={className}
    />
  );
}
