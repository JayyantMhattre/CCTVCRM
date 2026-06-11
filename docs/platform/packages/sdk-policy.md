# SDK policy

## Pin file

`BackEnd/global.json`:

```json
{
  "sdk": {
    "version": "10.0.103",
    "rollForward": "latestMinor"
  }
}
```

## Behavior

| Setting | Effect |
|---------|--------|
| `version` | Minimum SDK feature band required for the repo |
| `rollForward: latestMinor` | Allows newer **patch/minor** SDK on the same major (e.g. `10.0.300` satisfies `10.0.103`) |

## Developer setup

```bash
dotnet --version   # Should report 10.0.x
cd BackEnd
dotnet --info      # Confirms SDK from global.json
```

If SDK is missing, install [.NET 10 SDK](https://dotnet.microsoft.com/download) or use `rollForward` as documented above.

## CI

GitHub Actions workflow `ci.yml` installs `10.0.x` via `actions/setup-dotnet@v4` with `global-json-file: BackEnd/global.json`.

## Do not

- Remove `global.json` without team agreement
- Pin wildly different SDK versions in CI vs local without documenting in this file
