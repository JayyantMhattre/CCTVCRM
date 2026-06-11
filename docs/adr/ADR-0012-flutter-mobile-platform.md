# ADR-0012: Flutter as official mobile platform

## Status

Accepted (M0)

## Context

Ashraak has a .NET modular monolith backend and React 19 web client. Mobile clients are required for Android and iOS with feature parity over time.

Alternatives considered:

- **Flutter** — single Dart codebase, strong UI toolkit, good enterprise adoption
- **Native Kotlin + Swift** — maximum platform fidelity, dual codebases, higher cost
- **React Native** — share JS skills with web, different runtime from existing React SPA toolchain

## Decision

Adopt **Flutter** as the **only** official mobile stack for Android and iOS.

- Future app path: `FrontEnd.Mobile/`
- Mobile consumes same `/api/v1` and `/connect/token` as web
- Native Kotlin/Swift are **rejected** unless a future ADR approves a limited native module (e.g. platform channel)

## Rationale

| Factor | Flutter |
|--------|---------|
| Code sharing | One codebase for Android + iOS |
| Team alignment | Distinct from React web — clear boundary vs accidental coupling |
| Enterprise | Mature tooling, testing, CI |
| Backend parity | HTTP/OpenAPI clients straightforward |

Native dual-stack doubles maintenance. React Native would not reuse the Vite/React web bundle and adds a third UI paradigm.

## Consequences

- M0 delivers docs/governance only
- M1 creates Flutter project scaffold
- Mobile ADRs use `ADR-Mobile-*` prefix for client-specific decisions
- Platform manifest tracks Backend / Web / Mobile for every module
