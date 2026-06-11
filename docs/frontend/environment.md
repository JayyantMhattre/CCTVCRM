# Environment Configuration

## Files

| File | Committed | Purpose |
|------|-----------|---------|
| `apps/web/.env.example` | Yes | Template |
| `apps/web/.env.development` | Yes | Local defaults |
| `apps/web/.env.production` | **No** | Create at deploy time |

---

## Variables

| Variable | Required | Example | Used in |
|----------|----------|---------|---------|
| `VITE_API_BASE_URL` | Yes | `http://localhost:5000` | `client.ts`, Vite proxy target |
| `VITE_API_VERSION` | Yes | `v1` | `endpoints.ts` |
| `VITE_APP_NAME` | No | `Ashraak (Dev)` | Layout headers |

Only `VITE_*` variables are exposed to browser code.

---

## Vite proxy (development)

`vite.config.ts`:

- Dev server port: **3000**
- Proxies `/api` and `/connect` → `VITE_API_BASE_URL`

This allows frontend to call relative URLs in dev.

---

## Production build

```bash
VITE_API_BASE_URL=https://api.yourproduct.com pnpm build
```

Serve `dist/` behind CDN or static host; API on separate origin — configure CORS on backend if not using reverse proxy.

---

## TypeScript paths

`tsconfig.json`:

```json
"@/*": ["./src/*"]
```

---

## Related

- [getting-started/frontend-setup.md](../getting-started/frontend-setup.md)
- [getting-started/environment-variables.md](../getting-started/environment-variables.md)
