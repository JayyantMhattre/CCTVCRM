# Configuration validation — Troubleshooting

## Host fails immediately on start

Read the exception message — each line is a specific fix.

Example:

```
Environment validation failed...
 - ConnectionStrings:MongoDB is required.
```

Set `ConnectionStrings__MongoDB` in environment or `appsettings.Development.json`.

## Production signing key error

OpenIddict uses ephemeral keys in development. Production requires:

```bash
OpenIddict__SigningKey=<base64-or-secret-from-vault>
```

Or `Authentication__Jwt__SigningKey` if you wire custom JWT validation later.

## False positive in local Production profile

Do not set `ASPNETCORE_ENVIRONMENT=Production` locally without production secrets. Use `Development` for local work.
