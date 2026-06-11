# Theme Adapter Development Guide

**Audience:** Engineers implementing a theme adapter.
**Scope:** How to implement each of the **11 theme contracts** and assemble them into a registered `ThemeAdapter`.
**Source of truth:** `FrontEnd/apps/web/src/theme/contracts/*` (the TypeScript interfaces below are summarized from those files). When in doubt, the compiler against `ThemeAdapter` is the contract.

**Related:** [theme-onboarding-guide.md](./theme-onboarding-guide.md) · [theme-decision-record.md](./theme-decision-record.md) · reference implementations: `src/theme/adapters/coreui/` (default) and `src/theme/adapters/hexadash/` (opt-in).

---

## 0. Anatomy of an adapter

```
src/theme/adapters/<id>/
├── tokens/<id>.tokens.scss     # CSS custom properties (--<id>-*), scoped under .<id>-theme; light + dark
├── <id>.scss                   # visual skin consuming the tokens; scoped under .<id>-theme
├── navIcons.ts                 # maps NavigationIconKey → the theme's icon glyphs
├── <Theme>Layout.tsx           # LayoutContract.Layout      (imports <id>.scss)
├── <Theme>AuthLayout.tsx       # LayoutContract.AuthLayout
├── <Theme>Nav.tsx              # NavigationContract.Nav
├── <Theme>Card.tsx             # CardContract.Card
├── <Theme>Table.tsx            # TableContract.Table
├── <Theme>Dialog.tsx           # DialogContract.Dialog
├── <Theme>NotificationViewport.tsx  # NotificationContract.Viewport
├── <Theme>Badge.tsx            # BadgeContract.Badge
├── <Theme>Avatar.tsx           # AvatarContract.Avatar
├── <Theme>Tabs.tsx             # TabsContract.Tabs
├── <Theme>Breadcrumb.tsx       # BreadcrumbContract.Breadcrumb
├── <Theme>Chart.tsx            # ChartContract.Chart
└── index.ts                    # assembles `export const <id>Adapter: ThemeAdapter = {...}`
```

The `ThemeAdapter` shape every adapter must satisfy:

```ts
export interface ThemeAdapter {
  id: ThemeId;
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
```

### Golden rules for every contract
1. **Render only.** Adapters receive a fully-resolved, presentational model and turn it into markup. No data fetching, no access decisions, no routing logic.
2. **Scope all styles** under `.<id>-theme` so an inactive adapter contributes no active CSS.
3. **Honour props you're given; don't invent state** the platform owns (active tab id, nav visibility, toast queue, dialog `show`).
4. **Accessibility is part of the contract:** semantic elements, `aria-*`, focus order.
5. **No vendor app code** — reuse the theme's *look*, not its Redux/router/services.

---

## 1. LayoutContract

```ts
interface LayoutContract {
  Layout: ComponentType<PlatformLayoutProps>;      // authenticated shell
  AuthLayout: ComponentType<PlatformAuthLayoutProps>; // unauthenticated shell
}
// PlatformLayoutProps: { navGroups, user, onLogout, appName, isDarkMode?, onToggleDarkMode?, children? }
// PlatformAuthLayoutProps: { appName, children? }
```

**Implement:**
- `Layout` — render sidebar + header + content region + footer. Render `<Outlet />` when `children` is absent. Render the nav by delegating to your `Nav` (pass `navGroups` through). Show `user` + a logout action wired to `onLogout`.
- `AuthLayout` — render a centered/branded auth shell; render `<Outlet />` when `children` is absent.
- Dark mode: either consume `isDarkMode`/`onToggleDarkMode`, **or** manage it internally (CoreUI does the latter). If your page interiors are styled by a global stylesheet keyed off an attribute (as today), mirror your toggle onto that attribute so shell + content stay in sync.

**Don't:** read auth state, guard routes, or filter nav — the platform already resolved all of that.

## 2. NavigationContract

```ts
interface NavigationContract { Nav: ComponentType<PlatformNavProps>; }
// PlatformNavProps: { groups: readonly NavigationGroup[]; ariaLabel?: string }
// NavigationGroup: { id, title?, visible, items }
// NavigationItem:  { id, label, to, icon?: NavigationIconKey, badge?, visible }
```

**Implement:** iterate `groups`; render only groups/items with `visible === true` (the full list is passed so you can preserve separators). Use a router link for `to` and reflect active state. Map `icon` via your `navIcons.ts` (`Record<NavigationIconKey, Glyph>`). Render `badge` using your themed badge styling. Wrap in a `<nav aria-label={ariaLabel}>`.

**Don't:** compute visibility, roles, or permissions — `visible` is precomputed.

## 3. CardContract

```ts
interface CardContract { Card: ComponentType<PlatformCardProps>; }
// PlatformCardProps: { title?, actions?, footer?, className?, bodyClassName?, children? }
```

**Implement:** an outer card element; render a header region only when `title` or `actions` are present (`actions` right-aligned); a body wrapping `children` (apply `bodyClassName`); a footer when `footer` is present. Pass `className` through to the outer element.

## 4. TableContract

```ts
interface TableContract { Table: PlatformTableComponent; }
// generic: <TRow>(props: PlatformTableProps<TRow>) => ReactElement | null
// PlatformTableProps<TRow>: { columns: PlatformTableColumn<TRow>[]; rows: TRow[]; rowKey; empty?; className? }
// PlatformTableColumn<TRow>: { key, header, render: (row) => ReactNode, className? }
```

**Implement:** a generic function component. Render `<thead>` from `columns[].header` (apply column `className`); render a `<tbody>` row per `rows` entry keyed by `rowKey(row)`, each cell via `columns[].render(row)`. When `rows` is empty, render `empty` (in a full-width cell). Keep it generic — do not constrain `TRow`.

## 5. DialogContract

```ts
interface DialogContract { Dialog: ComponentType<PlatformDialogProps>; }
// PlatformDialogProps: { show, onClose, title?, footer?, size?: 'sm'|'lg'|'xl', centered?, children? }
```

**Implement:** a controlled modal driven by `show`. Call `onClose` on backdrop click, Escape, and the close icon. Render `title` in the header, `children` in the body, `footer` (e.g. buttons) in the footer. Map `size` to your modal widths; default `centered` to `true`. Ensure focus trap + restore (reuse a vetted modal primitive such as `react-bootstrap/Modal` rather than hand-rolling).

## 6. NotificationContract

```ts
interface NotificationContract { Viewport: ComponentType<PlatformNotificationViewportProps>; }
// PlatformNotificationViewportProps = Record<string, never>  (no props today)
```

**Implement:** a viewport that visually presents the stack of toasts. **State lives in the platform** (`shared/ui/toast` — queue/dismiss/autoclose); your viewport only styles the presentation (position, enter/leave, colours per severity). Do not implement the queue.

## 7. BadgeContract

```ts
interface BadgeContract { Badge: ComponentType<PlatformBadgeProps>; }
// PlatformBadgeProps: { variant?: PlatformBadgeVariant, pill?, children, className? }
// variant: 'primary'|'secondary'|'success'|'danger'|'warning'|'info'|'light'|'dark' (default 'secondary')
```

**Implement:** map the **semantic** `variant` to your theme tokens/classes (never expect callers to pass vendor classes). Support `pill` (fully rounded). Render `children` as content. Default `variant` to `secondary`.

## 8. AvatarContract

```ts
interface AvatarContract { Avatar: ComponentType<PlatformAvatarProps>; }
// PlatformAvatarProps: { name, src?: string|null, size?: 'sm'|'md'|'lg', className? }
```

**Implement:** if `src` is present, render the image with an accessible label derived from `name`; otherwise render initials derived from `name` (handle 1- and 2-word names safely). Map `size` to dimensions. Default `size` to `md`.

## 9. TabsContract

```ts
interface TabsContract { Tabs: ComponentType<PlatformTabsViewProps>; }
// PlatformTabsViewProps: { items: readonly PlatformTabItem[], activeId, onChange:(id)=>void, ariaLabel?, className? }
// PlatformTabItem: { id, label, content, disabled? }
```

**Implement:** a `role="tablist"` strip rendering each `item.label` as a `role="tab"` with `aria-selected` for `activeId`, roving `tabIndex`, and `onClick`/keyboard → `onChange(id)`. Render the active item's `content` in a `role="tabpanel"`. Respect `disabled`. **Active state is owned by the platform** — never store it yourself.

## 10. BreadcrumbContract

```ts
interface BreadcrumbContract { Breadcrumb: ComponentType<PlatformBreadcrumbViewProps>; }
// PlatformBreadcrumbViewProps: { items: readonly PlatformBreadcrumbEntry[], ariaLabel?, className? }
// PlatformBreadcrumbEntry: { label, to? }
```

**Implement:** a `<nav aria-label={ariaLabel ?? 'Breadcrumb'}>` with an ordered trail. Entries **with** `to` render as router links; the entry **without** `to` is the current page → render non-interactive with `aria-current="page"`. Add separators visually (CSS), not as list items.

## 11. ChartContract

```ts
interface ChartContract { Chart: ComponentType<PlatformChartProps>; }
// PlatformChartProps: { type: ChartType, data: ChartData, options?: ChartOptions, title?, ariaLabel?, className?, children? }
// ChartType: 'line'|'bar'|'area'|'pie'|'donut'|'scatter'|'radar'
// ChartData: { categories, series: { name, data:number[], color? }[] }
```

**Implement:** render a themed **frame** (title, sizing from `options.height`, `role="img"` + `ariaLabel`). The platform ships **no charting library**: if `children` (a concrete chart element) is provided, render it inside the frame; otherwise render a neutral placeholder. If/when a charting backend is adopted, map `type`/`data`/`options` to it **inside the adapter only** — the contract stays library-agnostic.

---

## Assembling and registering

`index.ts`:

```ts
import type { ThemeAdapter } from '@/theme/contracts';
// ...import all <Theme>* components

export const <id>Adapter: ThemeAdapter = {
  id: '<id>',
  label: '<Human Label>',
  layout:       { Layout: <Theme>Layout, AuthLayout: <Theme>AuthLayout },
  navigation:   { Nav: <Theme>Nav },
  card:         { Card: <Theme>Card },
  dialog:       { Dialog: <Theme>Dialog },
  table:        { Table: <Theme>Table },
  notification: { Viewport: <Theme>NotificationViewport },
  badge:        { Badge: <Theme>Badge },
  avatar:       { Avatar: <Theme>Avatar },
  tabs:         { Tabs: <Theme>Tabs },
  breadcrumb:   { Breadcrumb: <Theme>Breadcrumb },
  chart:        { Chart: <Theme>Chart },
};
```

`config.ts`:

```ts
export type ThemeId = 'coreui' | 'hexadash' | '<id>';
export const KNOWN_THEME_IDS: readonly ThemeId[] = ['coreui', 'hexadash', '<id>'];
// DEFAULT_THEME_ID stays unchanged until an explicit promotion
```

`registry.ts`:

```ts
import { <id>Adapter } from './adapters/<id>';
const REGISTRY: Record<ThemeId, ThemeAdapter> = {
  coreui: coreUiAdapter,
  hexadash: hexadashAdapter,
  <id>: <id>Adapter,   // ← the only wiring line
};
```

---

## Token & SCSS conventions

- `tokens/<id>.tokens.scss`: declare `--<id>-*` custom properties under `.<id>-theme` (light) and `.<id>-theme[data-<id>-mode='dark']` (dark). Cover colours, typography, spacing, radius, shadows, layout dims, breakpoints.
- `<id>.scss`: `@use './tokens/<id>.tokens.scss';` then style `.<id>-*` classes **only** under `.<id>-theme`. Import this stylesheet from `<Theme>Layout.tsx` so it loads when (and only when) the theme is active.
- Apply the `.<id>-theme` wrapper class at the root of `Layout` and `AuthLayout` (and any portal'd surface like dialogs/toasts that render outside the shell DOM).

## Self-check before opening a PR

- [ ] `npm run type-check` passes (structural proof all 11 contracts are satisfied).
- [ ] `npm test` — registry test sees the new id; both/all adapters expose the full contract surface.
- [ ] All CSS scoped under `.<id>-theme`; default theme visually unchanged.
- [ ] No imports of vendor auth/router/Redux/services/demo pages.
- [ ] Layout renders `<Outlet />` when `children` omitted; Nav respects `visible`; Tabs/Dialog/Notification don't own platform state.
- [ ] Accessibility: roles, `aria-*`, focus order verified.
