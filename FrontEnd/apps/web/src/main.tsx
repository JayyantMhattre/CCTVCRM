/**
 * Application entry point.
 *
 * Import order matters — global styles MUST load before any component
 * so that component-scoped CSS can cascade over the base styles.
 *
 * Style stack:
 *  1. coreui.scss  — CoreUI v5 (Bootstrap 5 superset: grid, utilities,
 *                    components, sidebar, header, footer, dark mode, etc.)
 *                    Replaces the previous `bootstrap/dist/css/bootstrap.min.css`.
 *  2. bootstrap-icons CSS (loaded from CDN in index.html for zero-bundle cost)
 */

// ── 1. CoreUI global stylesheet (replaces plain Bootstrap) ────────────────
// Vite compiles this SCSS on-the-fly in dev mode and into a single optimised
// CSS bundle in production.  The `sass` devDependency drives the compilation.
import './styles/coreui.scss';

// ── 2. React ───────────────────────────────────────────────────────────────
import { StrictMode }   from 'react';
import { createRoot }   from 'react-dom/client';
import { App }          from './App';

const root = document.getElementById('root');
if (!root) throw new Error('Root element #root not found in index.html.');

createRoot(root).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
