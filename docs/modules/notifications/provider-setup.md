# Notifications — Provider Setup

## Development (default)

`Notifications:Provider` = `console`

Emails appear in:

- Application console
- Serilog / Seq (`EMAIL to=...`)

## SMTP

Set `Provider` to `smtp` and configure `Notifications:Smtp` in environment or `appsettings.Production.json`.

No SendGrid/SES package dependency — implement `IEmailProvider` and register in `NotificationService.ResolveProvider` when needed.

## Seq

Console provider uses structured logging — filter `@mt` containing `EMAIL`.
