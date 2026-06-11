# ADR-0006: Notifications Observer Module

**Status:** Accepted  
**Date:** 2026-05-31

## Decision

Add Layer 3 **Notifications** module with `INotificationService` contract, file-based templates, and configuration-driven `IEmailProvider` (console/SMTP).

## Rationale

Decouples email from Auth/Users/Tenant; matches Audit observer pattern; supports future SendGrid/SES without module references.

## Tradeoffs

- `TenantProvisioned` uses placeholder recipient until owner email resolution
- `sendgrid`/`ses` keys fall back to console until implemented
