# Dependency governance

## Ownership

| Area | Owner file | Examples |
|------|------------|----------|
| Platform / host | `Directory.Packages.props` → ASP.NET, Serilog, OTel, health | Host team |
| Data | EF, Npgsql, Dapper, Mongo | Data layer |
| Auth | OpenIddict, Identity, Argon2, Otp.NET | Auth module |
| Testing | xUnit, FluentAssertions, Testcontainers | All contributors |

## Upgrade workflow

1. Identify package in `Directory.Packages.props`.
2. Check release notes for breaking changes (especially EF, OpenIddict).
3. Update **one logical group** at a time (e.g. all `Microsoft.Extensions.*` to same patch).
4. Run locally:
   ```bash
   cd BackEnd
   dotnet restore Ashraak.slnx
   dotnet build Ashraak.slnx --no-restore
   dotnet test Ashraak.slnx --no-build
   ```
5. Open PR; CI must pass restore, build, test, docs validation.

## Drift prevention

- PRs that add `PackageReference` must add central `PackageVersion`.
- CI `package-audit` job fails if any reference lacks a central version.
- Orphan central entries (unused pins) are allowed for planned features; document in [package-audit.md](./package-audit.md).

## Security advisories

`Directory.Build.props` suppresses `NU1902`–`NU1904` at build time; review advisories separately via:

```bash
dotnet list package --vulnerable
```
