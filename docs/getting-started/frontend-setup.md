# Frontend Setup

**Stack:** React 19 + Vite 6 (not Angular).

## Prerequisites

| Tool | Version |
|------|---------|
| Node.js | 20+ |
| pnpm | 9+ |

## Install

```bash
cd FrontEnd
pnpm install
```

## Environment

```bash
cd apps/web
Copy-Item .env.example .env.development
```

| Variable | Default | Purpose |
|----------|---------|---------|
| `VITE_API_BASE_URL` | `http://localhost:5000` | API origin |
| `VITE_API_VERSION` | `v1` | URL segment |
| `VITE_APP_NAME` | Product name in UI | |

## Run dev server

```bash
cd FrontEnd
pnpm dev
```

- App: `http://localhost:3000`
- Vite proxies `/api` and `/connect` to `VITE_API_BASE_URL`

## Build

```bash
pnpm build
pnpm type-check
```

## Related

- [frontend/architecture.md](../frontend/architecture.md)
- [local-development.md](./local-development.md)
