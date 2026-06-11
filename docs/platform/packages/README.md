# Package governance

Ashraak uses **Central Package Management (CPM)** for all NuGet dependencies.

| Document | Purpose |
|----------|---------|
| [central-package-management.md](./central-package-management.md) | `Directory.Packages.props` rules |
| [sdk-policy.md](./sdk-policy.md) | `global.json` and SDK roll-forward |
| [dependency-governance.md](./dependency-governance.md) | Upgrade workflow and ownership |
| [troubleshooting.md](./troubleshooting.md) | Restore/build failures |
| [package-audit.md](./package-audit.md) | Latest audit snapshot |

**Single source of versions:** `BackEnd/Directory.Packages.props`

**SDK pin:** `BackEnd/global.json`
