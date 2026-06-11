# HexaDash — Component Analysis

**Source root:** `hexadash-react/hexadash-react/src/components/` and `src/container/`

---

## Component organization

### `src/components/` — reusable primitives

| Folder | Purpose | Primary dependency |
|--------|---------|-------------------|
| `alerts` | Alert banners | Ant Design |
| `autoComplete` | Autocomplete input | Ant Design |
| `banners` | Marketing banners | styled-components |
| `buttons` | Button variants, export/share/calendar | Ant + custom |
| `cards` | Card frame wrapper (`cards-frame.js`) | styled-components |
| `cascader` | Cascading select | Ant Design |
| `charts` | Small chart wrappers | Mixed |
| `checkbox` | Checkbox groups | Ant Design |
| `color-palette` | Color swatch display | Custom |
| `comments` | Comment thread UI | Custom |
| `datePicker` | Date pickers | Ant Design |
| `drawer` | Slide-out panels | Ant Design |
| `dropdown` | Dropdown menus | Ant Design |
| `header` / `header-search` | Header sub-components | Custom |
| `heading` | Typography headings | Custom |
| `maps` | Map embeds | Leaflet / Google |
| `modals` | Modal wrapper | Ant Design Modal + styled |
| `note` | Note cards | Custom |
| `page-headers` | Breadcrumb page headers | Ant Design PageHeader |
| `popup` | Popover wrapper | Ant Design |
| `pricing` | Pricing tables | Custom |
| `slider` | Range sliders | Ant Design |
| `social-media` | Social share buttons | Custom |
| `steps` | Step wizards | Ant Design |
| `table` | Data table with filters | Ant Design Table |
| `tabs` | Tab panels | Ant Design |
| `tags` | Tag inputs | Ant Design |
| `tasklist` | Task list UI | Custom |
| `todo` | Todo item UI | Custom |
| `utilities` | AuthInfo, ProtectedRoute, ErrorBoundary, Search | Mixed |

### `src/container/` — page-level compositions

357 JavaScript files organized by feature domain (dashboard, ecommerce, chat, profile, etc.). Each container assembles components + demo data + Redux dispatchers.

---

## Dashboard pages

**Route module:** `routes/admin/dashboard.js`

| Route | Container | Description |
|-------|-----------|-------------|
| `/admin` | `container/dashboard` (index) | Default dashboard — Demo One |
| `/admin/demo-2` … `demo-10` | `DemoTwo` … `DemoTen` | Alternate layouts |

Each demo combines:

- Stat cards (`components/cards`)
- Chart widgets (ApexCharts / Chart.js inline)
- Tables, timelines, activity feeds
- Static data from `demoData/`

**No data fetching pattern** — all hard-coded or Redux demo slices.

Ashraak `DashboardPage` is minimal placeholder — HexaDash demos are **visual references only**.

---

## Table components

### Routes

| Path | Container | Component |
|------|-----------|-----------|
| `/admin/tables/basic` | `container/table/Table` | Ant `Table` basic |
| `/admin/tables/dataTable` | `container/table/DataTable` | Filterable data table |

### `components/table/DataTable.js`

| Feature | Implementation |
|---------|----------------|
| Table | Ant Design `Table` |
| Filters | Id input, status `Select` |
| Live filter | Redux `dataLiveFilter` dispatch |
| Submit filter | Redux `filterWithSubmit` |
| Selection | `rowSelection` prop passthrough |
| Styling | `DataTableStyleWrap`, `TableWrapper` |

**Tight coupling:** Filter logic uses `document.querySelector` for DOM values and Redux `data-filter` slice — not suitable for TanStack Query tables.

### Ashraak tables

User list, audit log, API keys, webhooks use custom page components with CoreUI table classes. Migrating to Ant `Table` would require a wrapper or full component swap.

---

## Form components

### Demo routes — `routes/admin/features.js` and `components.js`

| Path | Form type |
|------|-----------|
| `/admin/components/form` | Ant Design Form showcase |
| `/admin/features/form` | Layout variants (horizontal, vertical, multi-column) |

### Example — `container/forms/overview/HorizontalForm.js`

```javascript
<Form name="horizontal-form" layout="horizontal">
  <Row align="middle">
    <Col lg={8}><label>Name</label></Col>
    <Col lg={16}><Form.Item><Input /></Form.Item></Col>
  </Row>
</Form>
```

| Aspect | HexaDash | Ashraak |
|--------|----------|---------|
| Form library | Ant Design Form | React Hook Form |
| Validation | Ant rules / manual | Zod schemas |
| Layout | Ant `Row`/`Col` | Bootstrap/CoreUI grid |
| Default values | `initialValue` on items | `defaultValues` in RHF |

**Recommendation:** Keep Ashraak RHF + Zod. Optionally restyle inputs to match HexaDash tokens.

---

## Modal components

### `components/modals/antd-modals.js`

Thin wrapper over styled Ant Design `Modal`:

| Prop | Purpose |
|------|---------|
| `visible` / `open` | Visibility |
| `title`, `footer` | Custom footer with Cancel/Save buttons |
| `type`, `color` | Button variant passthrough |
| `width`, `className` | Sizing/styling |

Uses custom `Button` from `components/buttons/buttons.js` (not raw Ant Button).

### Demo page

`/admin/components/modals` — `container/ui-elements/Modals.js` showcases alert, confirm, nested modals.

Ashraak uses CoreUI modals or inline dialogs sparingly. Modal wrapper is **replaceable** via adapter if Ant Design is adopted.

---

## Chart components

| Library | Entry route | Container path |
|---------|-------------|----------------|
| Chart.js | `/admin/charts/chartjs` | `container/charts/ChartJs` |
| ApexCharts | `/admin/charts/apexcharts` | `container/charts/ApexCharts` |
| Recharts | `/admin/charts/recharts/*` | `container/charts/recharts/*` |
| Google Charts | `/admin/charts/google-chart` | `container/charts/GoogleCharts` |

Sub-routes for Recharts: bar, area, composed, line, pie, radar, radial.

**Bundle impact:** All four libraries together add significant weight. Ashraak should pick **one** chart library if dashboards are built — Recharts or ApexCharts align best with React 19.

---

## Authentication UI components

| Page | File | Features |
|------|------|----------|
| Sign In | `authentication/overview/SignIn.js` | Email/password, social links, link to register/forgot |
| Sign Up | `Signup.js` | Registration form |
| Forgot Password | `ForgotPassword.js` | Email reset form |
| Firebase Sign In | `FbSignIn.js` | Firebase provider buttons |
| Firebase Sign Up | `FbSignup.js` | Firebase registration |

Common patterns:

- `Cards` frame wrapper
- Ant `Form` with `onFinish` → Redux thunk
- Styled via `authentication/overview/style.js`
- Background illustration from `static/img/auth/`

Ashraak `LoginPage` / `RegisterPage` have MFA, tenant context, and API integration — **logic must stay**; only layout/CSS may borrow from HexaDash.

---

## Other notable component areas

| Area | Routes | Notes |
|------|--------|-------|
| UI Elements | `/admin/components/*` | 50+ Ant Design control demos |
| Widgets | `/admin/widgets/*` | KPI cards, charts |
| Email | `/admin/email/*` | Full inbox UI (Redux) |
| Chat | `/admin/main/chat/*` | Messaging UI (Redux) |
| Calendar | `/admin/app/calendar/*` | `react-big-calendar` |
| Kanban | Commented out | `@dnd-kit` available in deps |
| Maps | `/admin/maps/*` | Leaflet, Google Maps |
| Editor | `/admin/editor` | `@uiw/react-md-editor` |
| Import/Export | `/admin/importExport/*` | ExcelJS, xlsx-js-style |

---

## Shared patterns across components

### Card frame

`components/cards/frame/cards-frame.js` — standard page section:

```javascript
<Cards title="Section Title" bordered>
  {children}
</Cards>
```

Used on nearly every page for consistent padding, title, and shadow.

### Page headers

`components/page-headers/` — breadcrumb + title bar above card content.

### Buttons

`components/buttons/buttons.js` — themed button with `type`, `size`, `transparented` props wrapping Ant Button.

### Styled container

`container/styled.js` exports `Main`, `BasicFormWrapper`, `TableWrapper` — layout spacing wrappers.

---

## Component reuse matrix

| Component area | Reuse directly | Wrap in adapter | Do not use |
|----------------|----------------|-----------------|------------|
| Card frame visual | — | ✓ (reimplement with CoreUI card) | |
| DataTable + Redux filters | | | ✓ |
| Ant Form demos | | | ✓ (keep RHF) |
| Modal wrapper | | ✓ | |
| Chart demos | ✓ (pick one lib) | | |
| Auth page layout/CSS | | ✓ (visual only) | |
| AuthInfo header widgets | | ✓ (strip demo data) | |
| Firebase/Auth0 auth | | | ✓ |
| Ecommerce/chat/email containers | | | ✓ |
| UI element showcase pages | | | ✓ |
| Dashboard stat layouts | | ✓ (visual reference) | |
| Button variants | | ✓ | |
| Page headers / breadcrumbs | | ✓ | |

---

## TypeScript gap

All HexaDash components are **untyped JavaScript**. Ashraak requires TypeScript strict mode. Any adopted component must be:

1. Re-written in `.tsx` with proper interfaces, or
2. Wrapped with typed facade components, or
3. Used only as CSS/SCSS reference
