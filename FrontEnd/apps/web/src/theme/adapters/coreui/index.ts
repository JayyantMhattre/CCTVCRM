/**
 * CoreUI theme adapter — the platform's first (default) theme.
 *
 * Assembles the individual CoreUI surface implementations into a single
 * `ThemeAdapter` that satisfies every contract. Registered in `registry.ts`.
 */

import type { ThemeAdapter } from '@/theme/contracts';
import { CoreUiLayout } from './CoreUiLayout';
import { CoreUiAuthLayout } from './CoreUiAuthLayout';
import { CoreUiNav } from './CoreUiNav';
import { CoreUiCard } from './CoreUiCard';
import { CoreUiDialog } from './CoreUiDialog';
import { CoreUiTable } from './CoreUiTable';
import { CoreUiNotificationViewport } from './CoreUiNotificationViewport';
import { CoreUiBadge } from './CoreUiBadge';
import { CoreUiAvatar } from './CoreUiAvatar';
import { CoreUiTabs } from './CoreUiTabs';
import { CoreUiBreadcrumb } from './CoreUiBreadcrumb';
import { CoreUiChart } from './CoreUiChart';

export const coreUiAdapter: ThemeAdapter = {
  id: 'coreui',
  label: 'CoreUI 5',
  layout: {
    Layout: CoreUiLayout,
    AuthLayout: CoreUiAuthLayout,
  },
  navigation: {
    Nav: CoreUiNav,
  },
  card: {
    Card: CoreUiCard,
  },
  dialog: {
    Dialog: CoreUiDialog,
  },
  table: {
    Table: CoreUiTable,
  },
  notification: {
    Viewport: CoreUiNotificationViewport,
  },
  badge: {
    Badge: CoreUiBadge,
  },
  avatar: {
    Avatar: CoreUiAvatar,
  },
  tabs: {
    Tabs: CoreUiTabs,
  },
  breadcrumb: {
    Breadcrumb: CoreUiBreadcrumb,
  },
  chart: {
    Chart: CoreUiChart,
  },
};
