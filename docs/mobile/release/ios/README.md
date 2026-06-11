# iOS release signing

## Strategy

- **Local:** Xcode automatic signing or Fastlane Match
- **CI:** macOS runner + certificates from GitHub secrets (optional)
- **Export:** `ios/ExportOptions.plist.example` → `ExportOptions.plist` (gitignored)

## Certificates

| Approach | Use case |
|----------|----------|
| Xcode automatic | Local dev |
| Fastlane Match | Team shared certs |
| CI secrets | `IOS_CERTIFICATE_BASE64`, `IOS_CERTIFICATE_PASSWORD` |

## Flavors

Configure four schemes in Xcode matching Android flavors. Bundle ID pattern:

- `com.ashraak.ashraak_mobile.dev`
- `com.ashraak.ashraak_mobile.qa`
- `com.ashraak.ashraak_mobile.uat`
- `com.ashraak.ashraak_mobile`

## Build

```bash
./scripts/build-mobile.sh qa ipa
```

Or Fastlane: `bundle exec fastlane ios testflight flavor:qa`

Workflow: `.github/workflows/ios-release.yml`
