# Correlation — Tracing flow

```mermaid
sequenceDiagram
    participant Client
    participant API as Ashraak.Api
    participant Log as Serilog/Seq
    participant Audit as Audit module
    participant Outbox as Outbox processor

    Client->>API: Request (+ optional X-Correlation-Id)
    API->>API: CorrelationMiddleware sets baggage
    API->>Log: LogContext CorrelationId
    API->>Audit: API call log (same HTTP context)
    API->>Outbox: SaveChanges (async publish later)
    Outbox->>Log: Processor logs inherit ambient context when in-request
    API->>Client: Response + X-Correlation-Id
```

## Client usage

Send a stable ID per user action:

```http
X-Correlation-Id: 7f3c2a1b9e0d4f6a8b2c1d3e4f5a6b7c
```

Downstream support staff can search Seq by `CorrelationId` across login failures, audit entries, and outbox errors.
