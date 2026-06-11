# HexaDash — Dependency Analysis

**Source:** `hexadash-react/hexadash-react/package.json`  
**Compared against:** `FrontEnd/apps/web/package.json` (@ashraak/web)

---

## Build toolchain

| Package | HexaDash | Ashraak | Conflict |
|---------|----------|---------|----------|
| Build | `react-scripts` 5 + `@craco/craco` 7 | `vite` 6 | **High** — different bundler |
| React | 18.3.1 | 19.1.0 | **Medium** |
| TypeScript | None | 5.8.3 strict | **High** |
| Sass | 1.78.0 | 1.98.0 | Low — compatible |
| ESLint | 9.x (disabled in CRACO) | ESLint on `.ts/.tsx` | Medium |
| Test runner | Jest (CRA) | Vitest 3 | Medium |

HexaDash build requires `NODE_OPTIONS='--openssl-legacy-provider'` for production builds — indicates aging webpack/OpenSSL compatibility concerns.

---

## Core framework dependencies

| Package | HexaDash version | Ashraak version | Notes |
|---------|------------------|-----------------|-------|
| react | ^18.3.1 | ^19.1.0 | Major gap |
| react-dom | ^18.3.1 | ^19.1.0 | Major gap |
| react-router-dom | ^7.9.1 | ^7.5.3 | Compatible family |
| axios | ^1.7.7 | ^1.9.0 | Shared — reusable pattern |

---

## UI framework dependencies

| Package | HexaDash | Ashraak | Assessment |
|---------|----------|---------|------------|
| antd | ^5.27.3 | — | **HexaDash primary UI** |
| @ant-design/colors | ^7.2.1 (dev) | — | Ant token helpers |
| craco-less | ^3.0.1 (dev) | — | Ant Less compilation |
| styled-components | ^5.3.3 | — | Layout CSS-in-JS |
| @coreui/coreui | — | ^5.6.1 | **Ashraak primary UI** |
| @coreui/icons-react | — | ^2.3.0 | Ashraak icons |
| react-bootstrap | — | ^2.10.9 | Ashraak (CoreUI companion) |
| bootstrap-icons | — | ^1.11.3 | Ashraak icons |
| simplebar | — | ^6.3.3 | Ashraak scrollbar |

**Critical conflict:** Ant Design vs CoreUI — mutually exclusive as primary UI kits without significant bundle bloat and style conflicts.

---

## State management

| Package | HexaDash | Ashraak |
|---------|----------|---------|
| @reduxjs/toolkit | ^2.9.0 | — |
| react-redux | ^9.1.2 | — |
| redux-thunk | ^3.1.0 | — |
| react-redux-firebase | ^3.1.0 | — |
| redux-firestore | ^1.0.0 | — |
| zustand | — | ^5.0.4 |
| @tanstack/react-query | — | ^5.74.4 |

HexaDash Redux surface (~25 reducers) is demo scaffolding. **Do not merge into Ashraak.**

---

## Forms and validation

| Package | HexaDash | Ashraak |
|---------|----------|---------|
| react-hook-form | — | ^7.55.0 |
| @hookform/resolvers | — | ^5.0.1 |
| zod | — | ^3.24.3 |
| Ant Design Form | via antd | — |

---

## Authentication backends (HexaDash only)

| Package | Version | Purpose |
|---------|---------|---------|
| auth0-lock | ^14.1.0 | Auth0 embedded login |
| firebase | ^12.3.0 | Firebase SDK |
| react-redux-firebase | ^3.1.0 | Firebase auth state |
| js-cookie | ^3.0.5 | Cookie helpers |

Ashraak uses JWT via custom .NET API (`jwt-decode` ^4.0.0). Firebase/Auth0 deps are **not applicable**.

---

## Chart libraries (HexaDash only)

| Package | Version |
|---------|---------|
| apexcharts | ^5.3.5 |
| react-apexcharts | ^1.7.0 |
| chart.js | ^4.3.0 |
| react-chartjs-2 | ^5.1.0 |
| recharts | ^3.2.0 |
| react-google-charts | ^5.2.1 |

**Recommendation if charts needed in Ashraak:** Add one library (e.g. Recharts ^3.x) — do not import all four from HexaDash.

---

## Icons

| Package | HexaDash | Ashraak |
|---------|----------|---------|
| @iconscout/react-unicons | ^2.2.5 | — |
| feather-icons-react | ^0.9.0 | — |
| react-fontawesome | ^1.7.1 | — |
| Font Awesome 4.7 CDN | index.html | — |
| @coreui/icons | — | ^3.0.1 |

---

## Internationalization (HexaDash only)

| Package | Version |
|---------|---------|
| i18next | ^25.5.2 |
| react-i18next | ^15.0.1 |

Ashraak has no i18n dependencies.

---

## Date/time

| Package | HexaDash | Ashraak |
|---------|----------|---------|
| date-fns | ^3.6.0 | ^4.1.0 |
| dayjs | ^1.11.13 | — |

Both present in HexaDash (Ant Design uses dayjs internally in v5).

---

## Maps and geo (HexaDash only)

| Package | Version |
|---------|---------|
| leaflet | ^1.9.4 |
| react-leaflet | 4.2.1 |
| leaflet.markercluster | 1.5.3 |
| google-maps-react-18-support | ^2.1.0 |
| react-simple-maps | ^1.0.0 |

Not needed for current Ashraak feature set.

---

## Drag and drop / kanban (HexaDash only)

| Package | Version |
|---------|---------|
| @dnd-kit/core | ^6.1.0 |
| @dnd-kit/sortable | ^10.0.0 |
| react-dnd | ^16.0.1 |
| react-dnd-html5-backend | ^16.0.1 |

---

## Media / rich content (HexaDash only)

| Package | Version |
|---------|---------|
| swiper | ^12.0.1 |
| react-id-swiper | ^4.0.0 |
| react-lightbox-pack | ^0.2.1 |
| react-modal-video | ^2.0.2 |
| react-masonry-css | ^1.0.16 |
| @uiw/react-md-editor | ^4.0.8 |
| emoji-picker-react | ^4.12.0 |

---

## Data export (HexaDash only)

| Package | Version |
|---------|---------|
| exceljs | ^4.4.0 |
| file-saver | ^2.0.5 |
| react-csv | ^2.2.2 |
| xlsx-js-style | ^1.2.0 |

Potentially useful for audit log export — evaluate separately from theme adoption.

---

## Calendar / scheduling (HexaDash only)

| Package | Version |
|---------|---------|
| react-big-calendar | ^1.13.4 |
| react-calendar | ^5.1.0 |
| react-date-range | ^2.0.1 |

---

## UI utilities (HexaDash only)

| Package | Version | Purpose |
|---------|---------|---------|
| @pezhmanparsaee/react-custom-scrollbars | 4.3.1 | Sidebar scroll |
| react-countup | ^6.5.3 | Animated numbers |
| react-countdown | ^2.3.6 | Countdown widgets |
| react-tooltip | ^5.28.0 | Tooltips |
| react-tagsinput | ^3.20.3 | Tag input |
| react-svg | ^16.1.34 | SVG loader |
| immutability-helper | ^3.1.1 | State updates |
| array-move | ^4.0.0 | Sortable lists |
| prop-types | ^15.8.1 | Runtime types |
| ajv | ^8.17.1 | JSON schema |

---

## Dependency count summary

| Category | Approx. count |
|----------|---------------|
| HexaDash production deps | ~70 |
| Ashraak production deps | ~17 |
| Overlap (shared) | 4 (react, react-dom, react-router-dom, axios, date-fns, sass) |

---

## Version compatibility risks

| Risk | Detail | Mitigation |
|------|--------|------------|
| React 19 | HexaDash tested on React 18 | Spike Ant Design 5 + React 19 before adoption |
| styled-components 5 | Concurrent mode quirks with R19 | Prefer CSS variables / SCSS in adapter |
| react-redux-firebase 3.x | Unmaintained pattern | Do not import |
| react-scripts / CRA | Create React App is deprecated | Never migrate Ashraak back to CRA |
| auth0-lock 14 | Large bundle, iframe-based | Use Ashraak OAuth endpoints instead |
| Multiple chart libs | ~500KB+ combined | Select one |

---

## Recommended dependencies if adopting HexaDash visuals on CoreUI

**Add (optional, minimal):**

| Package | Reason |
|---------|--------|
| `recharts` OR `apexcharts` + `react-apexcharts` | Dashboard charts only |

**Do not add:**

- `antd` (unless committing to full UI migration)
- `redux` / `react-redux` (Ashraak has Zustand + Query)
- `firebase` / `react-redux-firebase`
- `styled-components` (conflicts with CoreUI SCSS approach)
- `react-scripts` / `@craco/craco`

---

## Alternative variant: hexadash-react-tailwind

Located at `C:\themeforest\hexadash-react-tailwind\`. Uses Tailwind CSS instead of Ant Design Less theming. Still conflicts with CoreUI SCSS but may offer **easier token extraction** (Tailwind config colors → CSS variables). Not analyzed in depth in this pass — consider if visual port to CoreUI is the goal.

---

## Bootstrap version (clarification)

Neither HexaDash React nor Ashraak lists `bootstrap` as a direct npm dependency.

| Project | CSS foundation |
|---------|----------------|
| HexaDash React | Ant Design 5 design tokens + custom SCSS |
| Ashraak | CoreUI 5 (Bootstrap 5-based SCSS) |

Bootstrap **5.x** is embedded in CoreUI for Ashraak. Bootstrap is **not present** in HexaDash React.
