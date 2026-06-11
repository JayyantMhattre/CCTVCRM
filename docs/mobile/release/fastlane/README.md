# Fastlane

**Path:** `FrontEnd.Mobile/fastlane/`

## Setup

```bash
cd FrontEnd.Mobile/fastlane
bundle install
```

## Android lanes

| Lane | Command |
|------|---------|
| build | `bundle exec fastlane android build flavor:qa` |
| beta | `bundle exec fastlane android beta flavor:qa` |
| production | `bundle exec fastlane android production` |

## iOS lanes

| Lane | Command |
|------|---------|
| build | `bundle exec fastlane ios build flavor:qa` |
| testflight | `bundle exec fastlane ios testflight flavor:qa` |
| production | `bundle exec fastlane ios production` |

## Environment variables

| Variable | Purpose |
|----------|---------|
| `FLAVOR` | dev / qa / uat / prod |
| `BUILD_NUMBER` | CI build number |
| `ASHRAAK_APP_IDENTIFIER` | iOS bundle id |
| `FASTLANE_TEAM_ID` | Apple team |
| `FASTLANE_APPLE_ID` | Apple ID for upload |

No secrets in `Appfile` — CI injects via secrets.
