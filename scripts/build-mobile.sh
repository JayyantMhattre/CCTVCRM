#!/usr/bin/env bash
# Build Ashraak mobile for a flavor (dev|qa|uat|prod).
set -euo pipefail

FLAVOR="${1:-dev}"
TARGET="${2:-apk}"
BUILD_NUMBER="${3:-0}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="${SCRIPT_DIR}/../FrontEnd.Mobile"

"${SCRIPT_DIR}/sync-mobile-version.sh" "$BUILD_NUMBER"
cd "$PROJECT_DIR"

DEFINES=(--dart-define=ENV="$FLAVOR" --dart-define=FLAVOR="$FLAVOR")
if [[ "$BUILD_NUMBER" != "0" ]]; then
  DEFINES+=(--dart-define=APP_BUILD_NUMBER="$BUILD_NUMBER")
fi

flutter pub get

case "$TARGET" in
  apk) flutter build apk --release --flavor "$FLAVOR" "${DEFINES[@]}" ;;
  appbundle) flutter build appbundle --release --flavor "$FLAVOR" "${DEFINES[@]}" ;;
  ipa) flutter build ipa --release --flavor "$FLAVOR" "${DEFINES[@]}" ;;
  ios) flutter build ios --release --flavor "$FLAVOR" "${DEFINES[@]}" ;;
  *) echo "Unknown target: $TARGET"; exit 1 ;;
esac

echo "Build complete: ${FLAVOR} / ${TARGET}"
