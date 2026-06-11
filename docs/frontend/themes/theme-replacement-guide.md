# Theme Replacement Guide

How to add and activate a new theme using the adapter foundation built in T1.

The whole point of the foundation: **a new theme is a new folder under `src/theme/adapters/` plus a one-line registration** — no changes to business modules, routing, guards, authentication, or state.

---

## Prerequisites

- The theme system from T1 is in place (`src/theme/`, `src/platform-ui/`).
- You know which surfaces your theme restyles (layout, nav, card, dialog, table, notifications).

---

## Step 1 — Extend the theme id

`src/theme/config.ts`:

```ts
export type ThemeId = 'coreui' | 'hexadash';

export const KNOWN_THEME_IDS: readonly ThemeId[] = ['coreui', 'hexadash'];
```

`DEFAULT_THEME_ID` normally stays `'coreui'` until the new theme is production-ready.

---

## Step 2 — Create the adapter folder

```
src/theme/adapters/hexadash/
├── HexadashLayout.tsx
├── HexadashAuthLayout.tsx
├── HexadashNav.tsx
├── HexadashCard.tsx
├── HexadashDialog.tsx
├── HexadashTable.tsx
├── HexadashNotificationViewport.tsx
├── hexadash.scss          # theme styles / tokens
└── index.ts               # assembles the ThemeAdapter
```

Each component implements its contract from `@/theme/contracts`. For example:

```tsx
// HexadashCard.tsx
import type { PlatformCardProps } from '@/theme/contracts';

export function HexadashCard({ title, actions, footer, children }: PlatformCardProps) {
  return (
    <div className="hd-card">
      {(title || actions) && (
        <div className="hd-card__head">
          <span>{title}</span>
          {actions}
        </div>
      )}
      <div className="hd-card__body">{children}</div>
      {footer && <div className="hd-card__foot">{footer}</div>}
    </div>
  );
}
```

`index.ts` assembles the adapter:

```ts
import type { ThemeAdapter } from '@/theme/contracts';
import { HexadashLayout } from './HexadashLayout';
import { HexadashAuthLayout } from './HexadashAuthLayout';
import { HexadashNav } from './HexadashNav';
import { HexadashCard } from './HexadashCard';
import { HexadashDialog } from './HexadashDialog';
import { HexadashTable } from './HexadashTable';
import { HexadashNotificationViewport } from './HexadashNotificationViewport';
import './hexadash.scss';

export const hexadashAdapter: ThemeAdapter = {
  id: 'hexadash',
  label: 'HexaDash',
  layout: { Layout: HexadashLayout, AuthLayout: HexadashAuthLayout },
  navigation: { Nav: HexadashNav },
  card: { Card: HexadashCard },
  dialog: { Dialog: HexadashDialog },
  table: { Table: HexadashTable },
  notification: { Viewport: HexadashNotificationViewport },
};
```

---

## Step 3 — Register it

`src/theme/registry.ts`:

```ts
import { hexadashAdapter } from './adapters/hexadash';

const REGISTRY: Record<ThemeId, ThemeAdapter> = {
  coreui: coreUiAdapter,
  hexadash: hexadashAdapter,
};
```

---

## Step 4 — Activate it

`.env.development` (or build env):

```
VITE_THEME=hexadash
```

Restart the dev server. `ThemeProvider` resolves and supplies the new adapter; every `platform-ui` component now renders the HexaDash surfaces.

---

## Step 5 — Validate replaceability

1. `tsc` type-check passes (the new adapter satisfies every contract).
2. Toggle `VITE_THEME` between `coreui` and `hexadash` — only presentation changes.
3. Permission-gated nav still hides items the user cannot access (logic is platform-side).
4. Auth, routing, and guards behave identically.
5. No file under `src/modules/` changed to add the theme.

---

## What you should NOT do

- ❌ Import a concrete adapter from `platform-ui` or `modules` — always go through the registry / `useTheme()`.
- ❌ Put permission or routing logic in an adapter.
- ❌ Fork `authStore`, guards, or `apiClient` for a theme.
- ❌ Add a vendor UI kit (e.g. Ant Design) to satisfy one surface — implement the contract with the chosen approach and keep the dependency isolated inside the adapter folder.

---

## Contract checklist for a new adapter

| Surface | Component | Source contract |
|---------|-----------|-----------------|
| App shell | `Layout` | `LayoutContract` |
| Auth shell | `AuthLayout` | `LayoutContract` |
| Sidebar nav | `Nav` | `NavigationContract` |
| Content card | `Card` | `CardContract` |
| Dialog | `Dialog` | `DialogContract` |
| Data table | `Table` | `TableContract` |
| Toast viewport | `Viewport` | `NotificationContract` |

---

## Related

- [theme-adapter-architecture.md](./theme-adapter-architecture.md)
- [theme-governance.md](./theme-governance.md)
- [current-theme/migration-recommendations.md](./current-theme/migration-recommendations.md)
