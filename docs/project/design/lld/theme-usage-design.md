# Theme Usage Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Mandate:** REUSE Theme Engine — **no theme-specific business screens** ([theme decision record](../../../frontend/themes/theme-decision-record.md))

---

## 1. Architecture rule (inviolable)

```
CCTV module pages → import ONLY from @/platform-ui and @/shared/*
                 → NEVER from @coreui/*, antd, or @/theme/adapters/*
```

Active theme (CoreUI default, HexaDash opt-in) is selected by `VITE_THEME` at build time. CCTV code must render identically on any adapter.

---

## 2. Theme compatibility rules

| Rule | Detail |
|------|--------|
| Primitives only | `PlatformCard`, `PlatformTable`, `PlatformFormField`, `PlatformDialog`, `PlatformBadge`, `PlatformTabs`, `PlatformBreadcrumb`, `PlatformLayout`, `PlatformPagination` |
| No raw vendor markup | No `CCard`, `CButton`, `antd Modal`, etc. in CCTV modules |
| No theme CSS imports | No `@coreui/coreui/dist/css` in CCTV module SCSS |
| Navigation | Register items in platform `navigationConfig.ts` — theme renders via `PlatformNavRenderer` |
| Icons | Use platform icon key map — not vendor icon components directly |
| Colors for status | Semantic variants (`success`, `danger`, `warning`, `info`) on `PlatformBadge` — not hard-coded hex in business code |
| Charts | Use `PlatformChart` contract only if charts added — no direct Recharts in modules |

---

## 3. Responsive rules (web portals)

| Breakpoint | Layout behavior |
|------------|-----------------|
| ≥1200px (desktop) | Full sidebar + 3-column dashboard grid |
| 768–1199px (tablet) | Collapsible sidebar + 2-column grid |
| <768px (mobile web) | Hidden sidebar (hamburger) + single column; tables horizontal scroll |

**PlatformLayout** handles sidebar collapse — CCTV pages do not implement custom layout shells.

### Form responsive

| Rule | Detail |
|------|--------|
| Two-column forms | `col-md-6` equivalent via platform form grid — single column on mobile |
| Wide tables | Horizontal scroll wrapper from `PlatformTable` |
| Visit evidence gallery | 2-col grid desktop, 1-col mobile |

---

## 4. Mobile rules (Flutter — separate from web theme)

| Rule | Detail |
|------|--------|
| Flutter theme | REUSE app `ThemeData` from platform mobile shell |
| No web theme engine | Flutter does not use `VITE_THEME` |
| Consistency | Match semantic colors (primary, error, success) to brand guidelines — not pixel-perfect with web |
| CCTV widgets | `VisitChecklistTile`, etc. use theme tokens — no hard-coded Material colors in business widgets |

---

## 5. Public website

| Approach | Detail |
|----------|--------|
| V1 | Marketing pages may reuse lightweight static layout or minimal `platform-ui` for inquiry forms |
| Login | REUSE platform `PlatformAuthLayout` + auth pages |
| Branding | Content from www.aarvii.in — outside theme adapter concern |

---

## 6. Future theme support

| Scenario | CCTV impact |
|----------|-------------|
| New ThemeForest theme | Zero CCTV code changes if new adapter implements 11 contracts |
| HexaDash activation | Set `VITE_THEME=hexadash` — CCTV pages inherit visuals automatically |
| T8 module migration | Platform migrates legacy module pages to `platform-ui`; CCTV built correctly from day one |
| Dark mode (future) | Theme adapter responsibility — CCTV uses semantic tokens only |

---

## 7. PDF vs theme

PDF documents ([pdf-document-design.md](./pdf-document-design.md)) use **fixed Aarvii brand layout** — independent of web theme adapters.

---

## 8. Verification checklist (PR review)

- [ ] No imports from `@coreui` or theme adapters in `modules/cctv-*`
- [ ] All pages wrapped in `PlatformLayout` (except public marketing)
- [ ] Status colors use `PlatformBadge` variants
- [ ] Navigation items added to platform config, not hard-coded in theme
- [ ] Architecture test passes layer boundaries

---

## 9. Classification

| Item | Class |
|------|:-----:|
| Theme Engine + adapters | REUSE |
| platform-ui primitives | REUSE |
| CCTV navigation entries | EXTEND platform nav config |
| CCTV page content | NEW (on platform-ui only) |
| Theme-specific CCTV screens | **Forbidden** |

---

Related: [platform-component-reuse.md](./platform-component-reuse.md) · [screen-design-specification.md](./screen-design-specification.md)
