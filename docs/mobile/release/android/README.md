# Android release signing

## Strategy

Release builds use `android/key.properties` + upload keystore. Debug signing used when `key.properties` is absent (CI smoke builds).

## Local setup

1. Generate keystore (once):

```bash
keytool -genkey -v -keystore ashraak-upload.jks -alias ashraak-upload -keyalg RSA -keysize 2048 -validity 10000
```

2. Copy `android/key.properties.example` → `android/key.properties`
3. Place `ashraak-upload.jks` in `android/keystore/` (gitignored)

## CI (GitHub secrets)

| Secret | Purpose |
|--------|---------|
| `ANDROID_KEYSTORE_BASE64` | Base64-encoded `.jks` |
| `ANDROID_KEYSTORE_PASSWORD` | Keystore password |
| `ANDROID_KEY_PASSWORD` | Key password |
| `ANDROID_KEY_ALIAS` | Key alias |

Workflow: `.github/workflows/android-release.yml`

## Artifacts

| Command | Output |
|---------|--------|
| `build-mobile.sh qa apk` | `build/app/outputs/flutter-apk/` |
| `build-mobile.sh prod appbundle` | `build/app/outputs/bundle/prodRelease/` |

ADR: [ADR-Mobile-0012](../../../adr/ADR-Mobile-0012-signing-strategy.md)
