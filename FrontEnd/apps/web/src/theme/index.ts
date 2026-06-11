/**
 * Theme system public API.
 *
 * Application code (and especially `platform-ui`) should import theme concerns
 * from here. Business modules must NOT import concrete adapters directly.
 */

export { ThemeProvider } from './ThemeProvider';
export { ThemeContext, useTheme } from './ThemeContext';
export type { ThemeContextValue } from './ThemeContext';

export { getThemeAdapter, getRegisteredThemeIds } from './registry';
export { resolveThemeId, DEFAULT_THEME_ID, KNOWN_THEME_IDS } from './config';
export type { ThemeId } from './config';

export type {
  ThemeAdapter,
  LayoutContract,
  NavigationContract,
  CardContract,
  DialogContract,
  TableContract,
  NotificationContract,
  BadgeContract,
  AvatarContract,
  TabsContract,
  BreadcrumbContract,
  ChartContract,
  PlatformLayoutProps,
  PlatformAuthLayoutProps,
  NavigationGroup,
  NavigationItem,
  NavigationBadge,
  NavigationBadgeVariant,
  NavigationIconKey,
  PlatformNavProps,
  PlatformCardProps,
  PlatformDialogProps,
  PlatformDialogSize,
  PlatformTableProps,
  PlatformTableColumn,
  PlatformNotificationViewportProps,
  PlatformBadgeProps,
  PlatformBadgeVariant,
  PlatformAvatarProps,
  PlatformAvatarSize,
  PlatformTabItem,
  PlatformTabsViewProps,
  PlatformBreadcrumbEntry,
  PlatformBreadcrumbViewProps,
  ChartType,
  ChartSeries,
  ChartData,
  ChartOptions,
  PlatformChartProps,
} from './contracts';
