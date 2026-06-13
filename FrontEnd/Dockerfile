# ─────────────────────────────────────────────────────────────────────────────
# Ashraak Web SPA — Multi-stage Dockerfile (React 19 + Vite 6)
#
# Stages:
#   base        — Node 20 + pnpm
#   development — Vite dev server with HMR (default for local testing)
#   build       — Production static asset build
#   production  — nginx serves compiled SPA
#
# Build context: FrontEnd/
# Usage:
#   docker build -t ashraak-web --target development .
#   docker build -t ashraak-web --target production .
# ─────────────────────────────────────────────────────────────────────────────

# ── Stage: base ───────────────────────────────────────────────────────────────
FROM node:20-alpine AS base
WORKDIR /app

RUN corepack enable \
    && corepack prepare pnpm@9.15.0 --activate

# Copy workspace manifests first for layer caching.
COPY package.json pnpm-workspace.yaml ./
COPY apps/web/package.json ./apps/web/

RUN pnpm install --filter @ashraak/web

# ── Stage: development ────────────────────────────────────────────────────────
FROM base AS development
WORKDIR /app

COPY apps/web ./apps/web

WORKDIR /app/apps/web
EXPOSE 3000

# --host 0.0.0.0 exposes Vite inside the container network.
CMD ["pnpm", "dev", "--host", "0.0.0.0", "--port", "3000"]

# ── Stage: build ──────────────────────────────────────────────────────────────
FROM base AS build
WORKDIR /app

COPY apps/web ./apps/web

WORKDIR /app/apps/web

ARG VITE_API_BASE_URL=http://localhost:8080
ARG VITE_API_VERSION=v1
ARG VITE_APP_NAME=Ashraak

ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}
ENV VITE_API_VERSION=${VITE_API_VERSION}
ENV VITE_APP_NAME=${VITE_APP_NAME}

RUN pnpm build

# ── Stage: production ─────────────────────────────────────────────────────────
FROM nginx:1.27-alpine AS production

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/apps/web/dist /usr/share/nginx/html

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD wget -qO- http://localhost:80/ > /dev/null 2>&1 || exit 1

CMD ["nginx", "-g", "daemon off;"]
