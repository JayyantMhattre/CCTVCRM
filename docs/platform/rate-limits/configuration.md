# Rate limiting — Configuration

## appsettings

```json
"RateLimiting": {
  "Enabled": true,
  "Routes": {
    "auth/token": {
      "Path": "/connect/token",
      "Methods": [ "POST" ],
      "Limit": 20,
      "WindowSeconds": 60
    },
    "auth/register": {
      "Path": "/api/v1/auth/register",
      "Methods": [ "POST" ],
      "Limit": 10,
      "WindowSeconds": 300
    }
  }
}
```

| Property | Description |
|----------|-------------|
| `Enabled` | Master switch; `false` disables middleware |
| `Routes` | Dictionary keyed by route group name |
| `Path` | URL prefix match (case-insensitive) |
| `Methods` | Optional HTTP methods; empty = all methods |
| `Limit` | Max requests per window |
| `WindowSeconds` | Fixed window length |

## Environment overrides

Use `RateLimiting__Enabled`, `RateLimiting__Routes__auth__token__Limit`, etc. (double underscore for nesting).

## Prerequisites

- Caching module registered first (`ICacheService` + Redis)
- `ConnectionStrings:Redis` reachable
