# Central package management

## Files

| File | Role |
|------|------|
| `BackEnd/Directory.Packages.props` | All `PackageVersion` definitions |
| `BackEnd/Directory.Build.props` | `ManagePackageVersionsCentrally`, `TargetFramework`, analyzers |
| `BackEnd/**/*.csproj` | `PackageReference` **without** `Version` attribute |

## Rules

1. **Never** add `Version="..."` on a project `PackageReference` — add or update `PackageVersion` centrally.
2. New package → add `PackageVersion` first, then reference from csproj.
3. Group related packages in `Directory.Packages.props` with comments (EF, Auth, Testing, etc.).
4. Prefer aligning `Microsoft.Extensions.*` and `Microsoft.AspNetCore.*` to the same major band as `TargetFramework` (`net10.0` → `10.0.x`).
5. EF Core remains on **9.x** intentionally (Npgsql/OpenIddict compatibility) — do not bump without a coordinated migration plan.

## NU1010

```
error NU1010: PackageReference items do not define a corresponding PackageVersion item
```

**Fix:** Add the missing package ID to `Directory.Packages.props`. Run the audit script in [package-audit.md](./package-audit.md).
