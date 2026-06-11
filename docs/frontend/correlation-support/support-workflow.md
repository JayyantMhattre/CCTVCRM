# Correlation — Support workflow

1. User reports a failed action (save settings, load audit, etc.).
2. Ask them to copy the **Correlation ID** from the error toast or red alert on the page.
3. Search Seq/backend logs with that ID (see [platform/correlation/seq-search.md](../../platform/correlation/seq-search.md)).

## Copy button

`CorrelationIdCopy` uses the Clipboard API with a brief “copied” confirmation icon.

## For developers

```ts
import { getLastCorrelationId } from '@/shared/errors/correlationId';

console.debug('Last API correlation', getLastCorrelationId());
```

Use after a failed mutation in devtools when the toast was dismissed.
