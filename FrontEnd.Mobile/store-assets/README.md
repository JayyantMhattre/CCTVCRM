# Store assets (M5)

Placeholder structure for Play Store and App Store submission assets.

**Do not commit production signing keys or live Firebase plist/json here.**

## Structure

```
store-assets/
├── android/
│   ├── icons/          # Adaptive launcher 512x512, feature graphic 1024x500
│   ├── screenshots/    # Phone + tablet captures per flavor if needed
│   └── listings/       # Short/full description, keywords (markdown drafts)
└── ios/
    ├── icons/          # App Store icon 1024x1024
    ├── screenshots/    # 6.7", 6.5", iPad if applicable
    └── listings/       # App name, subtitle, keywords, description
```

## Requirements checklist

See [docs/mobile/release/store-readiness.md](../../docs/mobile/release/store-readiness.md).
