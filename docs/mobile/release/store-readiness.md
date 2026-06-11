# Store readiness

Assets live in `FrontEnd.Mobile/store-assets/`.

## Required assets

### Android (Play Store)

| Asset | Spec |
|-------|------|
| App icon | 512×512 PNG |
| Feature graphic | 1024×500 |
| Screenshots | Phone (min 2), optional tablet |
| Short description | ≤ 80 chars |
| Full description | ≤ 4000 chars |
| Privacy policy | HTTPS URL |
| Content rating | Questionnaire complete |

### iOS (App Store)

| Asset | Spec |
|-------|------|
| App icon | 1024×1024 PNG (no alpha) |
| Screenshots | Per device class (6.7", etc.) |
| App name / subtitle | Character limits per locale |
| Privacy policy | URL in App Store Connect |
| App Privacy labels | Data collection disclosure |

## Splash / branding

Use `lib/shared/theme/` CoreUI-inspired colors — native splash configured in `android/app` and `ios/Runner` after bootstrap.

## Legal

- Privacy policy — must cover auth tokens, tenant data, optional analytics
- Terms of service — link in store listings
- Export compliance — standard HTTPS encryption declaration (iOS)

## Listing drafts

- Android: `store-assets/android/listings/README.md`
- iOS: `store-assets/ios/listings/README.md`
