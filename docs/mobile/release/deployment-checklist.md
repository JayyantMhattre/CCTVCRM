# Deployment checklist

## Pre-release

- [ ] `version.yaml` bumped (semver)
- [ ] `mobile-platform-manifest.md` current
- [ ] `flutter analyze` / `flutter test` pass
- [ ] All four flavors build locally or in CI
- [ ] Release notes generated
- [ ] OpenAPI/SDK drift checked if API changed

## Android

- [ ] `key.properties` / CI secrets configured for signed build
- [ ] AAB built: `./scripts/build-mobile.sh prod appbundle`
- [ ] Play Console listing updated (`store-assets/android/`)
- [ ] Privacy policy URL live

## iOS

- [ ] Certificates / profiles valid (Match or Xcode)
- [ ] IPA or TestFlight upload
- [ ] App Store Connect metadata (`store-assets/ios/`)
- [ ] Export compliance answered

## Post-release

- [ ] Manifest rows verified
- [ ] Monitor crash/analytics dashboards
- [ ] Rollback plan documented if needed
