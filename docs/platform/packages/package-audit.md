# Package audit (2026-05-31)

Automated comparison of all `PackageReference` entries in `BackEnd/**/*.csproj` against `BackEnd/Directory.Packages.props`.

## NU1010 root cause

Central Package Management (`ManagePackageVersionsCentrally=true`) requires every `PackageReference` to have a matching `PackageVersion` in `Directory.Packages.props`.

| Package | Referenced by |
|---------|----------------|
| `Microsoft.Extensions.Hosting.Abstractions` | `Ashraak.BuildingBlocks.Infrastructure` |
| `Microsoft.Extensions.Options` | `Ashraak.BuildingBlocks.Infrastructure`, `Ashraak.Notifications.Infrastructure` |

**Fix:** Added both at `10.0.0` (aligned with other `Microsoft.Extensions.*` entries).

## Additional build fixes (same phase)

| Issue | Fix |
|-------|-----|
| `Baggage` not found in host | Added `OpenTelemetry.Api` to host; aligned OpenTelemetry packages to `1.12.0` |
| NU1510 on Notifications.Infrastructure | Removed redundant `Microsoft.Extensions.*` refs (covered by `FrameworkReference`) |
| NU1605 OpenTelemetry downgrade | Bumped OTel central versions `1.11.2` → `1.12.0` as a set |

## Centrally pinned, not yet referenced

Reserved for planned features; safe to keep:

| Package | Notes |
|---------|--------|
| `MediatR.Extensions.Microsoft.DependencyInjection` | Legacy DI extension; MediatR 12 uses built-in registration |
| `Microsoft.Extensions.Http.Resilience` | Polly resilience for HTTP clients |
| `Sustainsys.Saml2.AspNetCore2` | SAML SSO scaffold |

## Audit command

```powershell
cd BackEnd
$central = Select-String Directory.Packages.props -Pattern 'PackageVersion Include="([^"]+)"' |
  ForEach-Object { $_.Matches.Groups[1].Value } | Sort-Object -Unique
$refs = Get-ChildItem -Recurse -Filter *.csproj |
  ForEach-Object { Select-String $_.FullName -Pattern 'PackageReference Include="([^"]+)"' |
    ForEach-Object { $_.Matches.Groups[1].Value } } | Sort-Object -Unique
$refs | Where-Object { $_ -notin $central }
```

Empty output = no NU1010 gaps.
