# Rollback strategy

## App Store / Play Store

1. **Halt rollout** — pause staged rollout in store console
2. **Previous version** — promote last known-good build from internal track
3. **Hotfix** — patch bump in `version.yaml`, fast-track QA flavor build

## API compatibility

Mobile is online-first. If backend breaking change shipped:

- Roll forward with patched client (preferred)
- Backend feature flags to disable incompatible endpoints

## Signing / build failures

- Re-run workflow with same tag + fixed secrets
- Never commit keystores or certificates to recover

## Communication

- Internal: incident channel + correlation IDs from crash reporter
- Users: store release notes for visible regressions

See [incident-response.md](./incident-response.md).
