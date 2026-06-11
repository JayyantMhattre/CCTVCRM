#!/usr/bin/env bash
# Syncs FrontEnd.Mobile/version.yaml -> pubspec.yaml
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="${SCRIPT_DIR}/../FrontEnd.Mobile"
VERSION_FILE="${PROJECT_DIR}/version.yaml"
PUBSPEC="${PROJECT_DIR}/pubspec.yaml"
BUILD_OVERRIDE="${1:-0}"

SEMVER=$(grep -E '^version:' "$VERSION_FILE" | awk '{print $2}')
BUILD=$(grep -E '^build:' "$VERSION_FILE" | awk '{print $2}')
if [[ "$BUILD_OVERRIDE" != "0" ]]; then BUILD="$BUILD_OVERRIDE"; fi

FULL="${SEMVER}+${BUILD}"
echo "Syncing mobile version -> ${FULL}"
sed -i.bak "s/^version:.*/version: ${FULL}/" "$PUBSPEC" && rm -f "${PUBSPEC}.bak"
echo "Updated pubspec.yaml"
