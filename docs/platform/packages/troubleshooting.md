# Package & restore troubleshooting

## NU1010 — missing central version

**Symptom:** Restore fails; package name listed in error.

**Fix:** Add to `BackEnd/Directory.Packages.props`:

```xml
<PackageVersion Include="Package.Id" Version="x.y.z" />
```

Re-run `dotnet restore`.

## SDK not found

**Symptom:** `A compatible .NET SDK was not found` + `global.json` path.

**Fix:**

1. Install .NET 10 SDK, or
2. Ensure installed SDK satisfies `global.json` with `rollForward` (see [sdk-policy.md](./sdk-policy.md)).

## NU1603 / NU1608 (version resolution)

Suppressed in `Directory.Build.props` via `NoWarn`. If restore still fails, align dependent package versions in `Directory.Packages.props` (especially EF + Npgsql).

## NU1510 (package pruning)

On `net10.0`, explicit `Microsoft.Extensions.*` references may be redundant when `FrameworkReference` to `Microsoft.AspNetCore.App` is present. Either remove redundant `PackageReference` entries or rely on `NU1510` in `NoWarn` (repo default).

## NullReference in NuGet.targets

Often follows NU1010 — fix central versions first, delete `obj/` and `bin/`, restore again:

```bash
cd BackEnd
dotnet nuget locals all --clear   # optional, last resort
dotnet restore Ashraak.slnx
```

## Build succeeds, host fails at runtime

Not a package issue — check [startup-troubleshooting.md](../../operations/startup-troubleshooting.md) and connection strings.

## Verify health

```bash
cd BackEnd
dotnet build Ashraak.slnx
dotnet test Ashraak.slnx
dotnet run --project src/Host/Ashraak.Api --no-build
curl http://localhost:5000/health/live
```
