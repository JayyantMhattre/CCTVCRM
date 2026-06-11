# Deployment Notes

## Artifacts

| Component | Artifact |
|-----------|----------|
| API | Docker image from `BackEnd/Dockerfile` → `Ashraak.Api.dll` |
| Frontend | Static `FrontEnd/apps/web/dist/` |

---

## Production compose

```bash
cd BackEnd
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

Prod overlay:

- Stricter logging levels
- Resource limits
- Secret placeholders — **must** replace `CHANGE_ME_*`

---

## Required secrets (production)

| Secret | Purpose |
|--------|---------|
| `POSTGRES_PASSWORD` | Database |
| `MONGO_PASSWORD` | Audit DB |
| `REDIS_PASSWORD` | Recommended |
| `RABBITMQ_PASSWORD` | If broker used later |
| `GOOGLE_CLIENT_SECRET` | SSO |
| `MICROSOFT_CLIENT_SECRET` | SSO |

`JWT_SIGNING_KEY_BASE64` — documented but **implement persistent signing before prod**.

---

## Reverse proxy

Place nginx or Traefik in front of API:

- TLS termination
- Route `/api`, `/connect`, `/health`, `/scalar` (disable Scalar in prod)
- Frontend static files on CDN or same proxy

---

## Readiness gates

Only route traffic when:

```
GET /health/ready → 200
```

---

## Frontend deploy

Build with production API URL:

```bash
VITE_API_BASE_URL=https://api.example.com pnpm build
```

Serve `dist/` with cache headers; SPA fallback to `index.html`.

---

## Deep reference

[DOCKER_ENVIRONMENT.md](../../BackEnd/DOCKER_ENVIRONMENT.md)

---

## Related

- [environment-variables.md](../getting-started/environment-variables.md)
- [documentation-governance.md](../documentation-governance.md)
