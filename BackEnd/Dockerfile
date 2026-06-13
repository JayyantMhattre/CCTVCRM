# ─────────────────────────────────────────────────────────────────────────────
# Ashraak API — Multi-stage Dockerfile
#
# Stages:
#   1. base    — lightweight ASP.NET 10 runtime image
#   2. build   — SDK image; restores and builds the entire solution
#   3. publish — creates the final self-contained output
#   4. final   — minimal runtime image used for deployment
#
# Build context: BackEnd/ (the directory containing this file)
# Usage:
#   docker build -t ashraak-api .
#   docker build -t ashraak-api --target build .    # stop at build stage for debugging
# ─────────────────────────────────────────────────────────────────────────────

# ── Stage 1: runtime base ─────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Expose the HTTP port the app listens on inside the container.
# The host port mapping is controlled in docker-compose, not here.
EXPOSE 8080

# curl is required by Docker HEALTHCHECK and compose health probes.
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user for security hardening.
# Running as root inside a container is a security risk.
RUN addgroup --system --gid 1001 appgroup \
    && adduser  --system --uid 1001 --ingroup appgroup --no-create-home appuser

# ── Stage 2: build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ── Layer-cache optimisation ──────────────────────────────────────────────────
# Copy ONLY the project files first.  Docker caches this layer and skips the
# expensive `dotnet restore` step on subsequent builds when only source files
# change (not dependencies).
#
# Pattern: copy each .csproj with its relative path preserved so the directory
# structure matches the COPY . . step below.

# Host
COPY ["src/Host/Ashraak.Api/Ashraak.Api.csproj",                                           "src/Host/Ashraak.Api/"]

# Shared
COPY ["src/Shared/Ashraak.SharedKernel/Ashraak.SharedKernel.csproj",                       "src/Shared/Ashraak.SharedKernel/"]
COPY ["src/Shared/Ashraak.SharedKernel.Contracts/Ashraak.SharedKernel.Contracts.csproj",   "src/Shared/Ashraak.SharedKernel.Contracts/"]

# BuildingBlocks
COPY ["src/BuildingBlocks/Ashraak.BuildingBlocks.Application/Ashraak.BuildingBlocks.Application.csproj", "src/BuildingBlocks/Ashraak.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/Ashraak.BuildingBlocks.EventBus/Ashraak.BuildingBlocks.EventBus.csproj",       "src/BuildingBlocks/Ashraak.BuildingBlocks.EventBus/"]
COPY ["src/BuildingBlocks/Ashraak.BuildingBlocks.Infrastructure/Ashraak.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Ashraak.BuildingBlocks.Infrastructure/"]

# Shared Infrastructure
COPY ["src/Infrastructure/Ashraak.Infrastructure.Shared/Ashraak.Infrastructure.Shared.csproj", "src/Infrastructure/Ashraak.Infrastructure.Shared/"]

# Auth module
COPY ["src/Modules/Auth/Ashraak.Auth.Domain/Ashraak.Auth.Domain.csproj",                   "src/Modules/Auth/Ashraak.Auth.Domain/"]
COPY ["src/Modules/Auth/Ashraak.Auth.Application/Ashraak.Auth.Application.csproj",         "src/Modules/Auth/Ashraak.Auth.Application/"]
COPY ["src/Modules/Auth/Ashraak.Auth.Infrastructure/Ashraak.Auth.Infrastructure.csproj",   "src/Modules/Auth/Ashraak.Auth.Infrastructure/"]
COPY ["src/Modules/Auth/Ashraak.Auth.Api/Ashraak.Auth.Api.csproj",                         "src/Modules/Auth/Ashraak.Auth.Api/"]

# Tenant module
COPY ["src/Modules/Tenant/Ashraak.Tenant.Domain/Ashraak.Tenant.Domain.csproj",             "src/Modules/Tenant/Ashraak.Tenant.Domain/"]
COPY ["src/Modules/Tenant/Ashraak.Tenant.Application/Ashraak.Tenant.Application.csproj",   "src/Modules/Tenant/Ashraak.Tenant.Application/"]
COPY ["src/Modules/Tenant/Ashraak.Tenant.Infrastructure/Ashraak.Tenant.Infrastructure.csproj", "src/Modules/Tenant/Ashraak.Tenant.Infrastructure/"]
COPY ["src/Modules/Tenant/Ashraak.Tenant.Api/Ashraak.Tenant.Api.csproj",                   "src/Modules/Tenant/Ashraak.Tenant.Api/"]

# Users module
COPY ["src/Modules/Users/Ashraak.Users.Domain/Ashraak.Users.Domain.csproj",               "src/Modules/Users/Ashraak.Users.Domain/"]
COPY ["src/Modules/Users/Ashraak.Users.Application/Ashraak.Users.Application.csproj",     "src/Modules/Users/Ashraak.Users.Application/"]
COPY ["src/Modules/Users/Ashraak.Users.Infrastructure/Ashraak.Users.Infrastructure.csproj", "src/Modules/Users/Ashraak.Users.Infrastructure/"]
COPY ["src/Modules/Users/Ashraak.Users.Api/Ashraak.Users.Api.csproj",                     "src/Modules/Users/Ashraak.Users.Api/"]

# Audit module
COPY ["src/Modules/Audit/Ashraak.Audit.Domain/Ashraak.Audit.Domain.csproj",               "src/Modules/Audit/Ashraak.Audit.Domain/"]
COPY ["src/Modules/Audit/Ashraak.Audit.Application/Ashraak.Audit.Application.csproj",     "src/Modules/Audit/Ashraak.Audit.Application/"]
COPY ["src/Modules/Audit/Ashraak.Audit.Infrastructure/Ashraak.Audit.Infrastructure.csproj", "src/Modules/Audit/Ashraak.Audit.Infrastructure/"]
COPY ["src/Modules/Audit/Ashraak.Audit.Api/Ashraak.Audit.Api.csproj",                     "src/Modules/Audit/Ashraak.Audit.Api/"]

# Caching module
COPY ["src/Modules/Caching/Ashraak.Caching.Abstractions/Ashraak.Caching.Abstractions.csproj", "src/Modules/Caching/Ashraak.Caching.Abstractions/"]
COPY ["src/Modules/Caching/Ashraak.Caching.Redis/Ashraak.Caching.Redis.csproj",           "src/Modules/Caching/Ashraak.Caching.Redis/"]

# Copy the central package management files (must exist before restore).
COPY ["Directory.Build.props",    "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json",              "."]

# Restore NuGet packages (uses the API project as entry point — it pulls all transitive deps).
RUN dotnet restore "src/Host/Ashraak.Api/Ashraak.Api.csproj"

# ── Now copy all remaining source code ───────────────────────────────────────
COPY . .

ARG BUILD_CONFIGURATION=Release

# Build the API project.
RUN dotnet build "src/Host/Ashraak.Api/Ashraak.Api.csproj" \
    -c "$BUILD_CONFIGURATION" \
    --no-restore \
    -o /app/build

# ── Stage 3: publish ──────────────────────────────────────────────────────────
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish to a self-contained directory.
# --no-build: build was already done in the previous stage.
RUN dotnet publish "src/Host/Ashraak.Api/Ashraak.Api.csproj" \
    -c "$BUILD_CONFIGURATION" \
    --no-build \
    -o /app/publish \
    /p:UseAppHost=false

# ── Stage 4: final runtime image ─────────────────────────────────────────────
FROM base AS final
WORKDIR /app

# Copy the published output from the publish stage.
COPY --from=publish /app/publish .

# Switch to non-root user (created in base stage).
USER appuser

# Health check — the API exposes /health/live.
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD curl --fail http://localhost:8080/health/live || exit 1

# Start the API.
ENTRYPOINT ["dotnet", "Ashraak.Api.dll"]
