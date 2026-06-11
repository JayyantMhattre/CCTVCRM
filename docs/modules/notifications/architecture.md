# Notifications — Architecture

Observer module (Layer 3) — no business aggregates. Reacts to contract events via MediatR.

```mermaid
sequenceDiagram
    participant Outbox as Outbox Processor
    participant Bridge as Domain Event Bridge
    participant MediatR
    participant Handler as NotificationEventHandler
    participant Svc as INotificationService
    participant Provider as IEmailProvider

    Outbox->>MediatR: Publish UserCreatedEvent
    MediatR->>Handler: Handle
    Handler->>Svc: SendEmailAsync(welcome)
    Svc->>Provider: SendAsync
```

## Provider model

| Provider key | Implementation | Use |
|--------------|----------------|-----|
| `console` | `ConsoleEmailProvider` | Development (default) |
| `smtp` | `SmtpEmailProvider` | Production SMTP |
| `sendgrid` / `ses` | Falls back to console until wired | Future |

Configuration: `Notifications` section in `appsettings.json`.

## Templates

File-based `Templates/{name}.txt` under host content root (`Ashraak.Api/Templates/`).

Format:

```
Subject: Line one
Body lines...
{{Placeholder}}
```
