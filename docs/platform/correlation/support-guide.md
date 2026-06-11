# Correlation — Support guide

## For support engineers

1. Ask the customer for the failing request’s `X-Correlation-Id` (browser devtools → Response Headers).
2. Search Seq with `CorrelationId = '<value>'`.
3. If missing, check whether a proxy stripped headers.

## For API consumers

- Propagate the same correlation ID on retries of the **same** logical operation.
- Generate a **new** ID for a new user action.

## Privacy

Correlation IDs are opaque identifiers — do not embed PII in the header value.
