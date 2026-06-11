/**
 * Theme contracts barrel.
 *
 * A `ThemeAdapter` is the complete set of surfaces a theme must implement.
 * The platform-ui layer renders against these contracts only — never against a
 * concrete theme — which is what makes themes replaceable.
 */

import type { ThemeId } from '../config';
import type { LayoutContract } from './LayoutContract';
import type { NavigationContract } from './NavigationContract';
import type { CardContract } from './CardContract';
import type { DialogContract } from './DialogContract';
import type { TableContract } from './TableContract';
import type { NotificationContract } from './NotificationContract';
import type { BadgeContract } from './BadgeContract';
import type { AvatarContract } from './AvatarContract';
import type { TabsContract } from './TabsContract';
import type { BreadcrumbContract } from './BreadcrumbContract';
import type { ChartContract } from './ChartContract';

/** The full contract a theme adapter must satisfy. */
export interface ThemeAdapter {
  /** Theme identifier (matches the registry key). */
  id: ThemeId;
  /** Human-readable label (for diagnostics / theme pickers). */
  label: string;
  layout: LayoutContract;
  navigation: NavigationContract;
  card: CardContract;
  dialog: DialogContract;
  table: TableContract;
  notification: NotificationContract;
  badge: BadgeContract;
  avatar: AvatarContract;
  tabs: TabsContract;
  breadcrumb: BreadcrumbContract;
  chart: ChartContract;
}

export type {
  LayoutContract,
  PlatformLayoutProps,
  PlatformAuthLayoutProps,
  PlatformLayoutComponent,
  PlatformAuthLayoutComponent,
} from './LayoutContract';

export type {
  NavigationContract,
  NavigationGroup,
  NavigationItem,
  NavigationBadge,
  NavigationBadgeVariant,
  NavigationIconKey,
  PlatformNavProps,
  PlatformNavComponent,
} from './NavigationContract';

export type {
  CardContract,
  PlatformCardProps,
  PlatformCardComponent,
} from './CardContract';

export type {
  DialogContract,
  PlatformDialogProps,
  PlatformDialogSize,
  PlatformDialogComponent,
} from './DialogContract';

export type {
  TableContract,
  PlatformTableProps,
  PlatformTableColumn,
  PlatformTableComponent,
} from './TableContract';

export type {
  NotificationContract,
  PlatformNotificationViewport,
  PlatformNotificationViewportProps,
} from './NotificationContract';

export type {
  BadgeContract,
  PlatformBadgeProps,
  PlatformBadgeVariant,
  PlatformBadgeComponent,
} from './BadgeContract';

export type {
  AvatarContract,
  PlatformAvatarProps,
  PlatformAvatarSize,
  PlatformAvatarComponent,
} from './AvatarContract';

export type {
  TabsContract,
  PlatformTabItem,
  PlatformTabsViewProps,
  PlatformTabsViewComponent,
} from './TabsContract';

export type {
  BreadcrumbContract,
  PlatformBreadcrumbEntry,
  PlatformBreadcrumbViewProps,
  PlatformBreadcrumbViewComponent,
} from './BreadcrumbContract';

export type {
  ChartContract,
  ChartType,
  ChartSeries,
  ChartData,
  ChartOptions,
  PlatformChartProps,
  PlatformChartComponent,
} from './ChartContract';
