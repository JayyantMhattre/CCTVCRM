#!/usr/bin/env bash
# Generates Dart API client from backend OpenAPI document.
set -euo pipefail

OPENAPI_URL="${1:-http://localhost:5000/openapi/v1.json}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTPUT_DIR="${SCRIPT_DIR}/../FrontEnd.Mobile/packages/api_client"
TEMP_SPEC="${TMPDIR:-/tmp}/ashraak-openapi-v1.json"
PACKAGE_NAME="ashraak_api_client"

if [[ -n "${OPENAPI_SPEC_FILE:-}" ]]; then
  cp "$OPENAPI_SPEC_FILE" "$TEMP_SPEC"
else
  echo "Fetching OpenAPI from $OPENAPI_URL"
  curl -fsSL "$OPENAPI_URL" -o "$TEMP_SPEC"
fi

command -v docker >/dev/null 2>&1 || {
  echo "Docker required. See docs/mobile/sdk-generation.md for alternatives."
  exit 1
}

mkdir -p "$OUTPUT_DIR"

docker run --rm \
  -v "$TEMP_SPEC:/local/openapi.json:ro" \
  -v "$OUTPUT_DIR:/local/out" \
  openapitools/openapi-generator-cli:v7.10.0 generate \
  -i /local/openapi.json \
  -g dart-dio \
  -o /local/out \
  --additional-properties=pubName="$PACKAGE_NAME",nullableFields=true

echo "SDK generation complete: $OUTPUT_DIR"
