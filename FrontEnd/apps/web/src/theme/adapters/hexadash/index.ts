/**
 * HexaDash theme adapter — the platform's second (opt-in) theme.
 *
 * Implements every `ThemeAdapter` contract using CoreUI/Bootstrap-compatible
 * primitives styled with the extracted HexaDash design tokens (see
 * `tokens/hexadash.tokens.scss`). It deliberately reuses NONE of HexaDash's
 * auth logic, routing, Redux, services, APIs, state, or demo pages.
 *
 * Registered (but NOT default) in `registry.ts`; activated via
 * `VITE_THEME=hexadash`. CoreUI remains the production default.
 */

import type { ThemeAdapter } from '@/theme/contracts';
import { HexaDashLayout } from './HexaDashLayout';
import { HexaDashAuthLayout } from './HexaDashAuthLayout';
import { HexaDashNav } from './HexaDashNav';
import { HexaDashCard } from './HexaDashCard';
import { HexaDashDialog } from './HexaDashDialog';
import { HexaDashTable } from './HexaDashTable';
import { HexaDashNotificationViewport } from './HexaDashNotificationViewport';
import { HexaDashBadge } from './HexaDashBadge';
import { HexaDashAvatar } from './HexaDashAvatar';
import { HexaDashTabs } from './HexaDashTabs';
import { HexaDashBreadcrumb } from './HexaDashBreadcrumb';
import { HexaDashChart } from './HexaDashChart';

export const hexadashAdapter: ThemeAdapter = {
  id: 'hexadash',
  label: 'HexaDash',
  layout: {
    Layout: HexaDashLayout,
    AuthLayout: HexaDashAuthLayout,
  },
  navigation: {
    Nav: HexaDashNav,
  },
  card: {
    Card: HexaDashCard,
  },
  dialog: {
    Dialog: HexaDashDialog,
  },
  table: {
    Table: HexaDashTable,
  },
  notification: {
    Viewport: HexaDashNotificationViewport,
  },
  badge: {
    Badge: HexaDashBadge,
  },
  avatar: {
    Avatar: HexaDashAvatar,
  },
  tabs: {
    Tabs: HexaDashTabs,
  },
  breadcrumb: {
    Breadcrumb: HexaDashBreadcrumb,
  },
  chart: {
    Chart: HexaDashChart,
  },
};
