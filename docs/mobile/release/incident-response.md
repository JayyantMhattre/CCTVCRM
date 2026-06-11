# Mobile release incident response

## Severity guide

| Level | Example | Action |
|-------|---------|--------|
| S1 | Auth broken in prod | Halt rollout, hotfix within hours |
| S2 | Crash spike post-release | Pause rollout, patch release |
| S3 | Non-critical UI defect | Next scheduled release |

## Triage

1. Confirm flavor/environment affected (dev vs prod)
2. Check crash reporting logs (`CrashReportingService`)
3. Correlate with backend health and API version
4. Identify build number from `AppVersion.fullVersion`

## Rollback

Follow [rollback-strategy.md](./rollback-strategy.md).

## Post-incident

- Update deployment checklist if gap found
- ADR amendment if architectural change required
- Manifest note if capability status changes
