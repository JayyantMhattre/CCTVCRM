#!/usr/bin/env bash
# Validates that module/feature code changes include documentation updates.
# Usage: ./scripts/validate-docs.sh [base-ref]
#   base-ref defaults to origin/main, then main, then HEAD~1
# Env: DOC_VALIDATE_MODE=warn|fail (overrides docs/doc-validation.json mode)

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"

CONFIG="$ROOT/docs/doc-validation.json"
BASE_REF="${1:-}"

if [[ -z "$BASE_REF" ]]; then
  for candidate in origin/main origin/master main HEAD~1; do
    if git rev-parse --verify "$candidate" >/dev/null 2>&1; then
      BASE_REF="$candidate"
      break
    fi
  done
fi

if [[ -z "$BASE_REF" ]] || ! git rev-parse --verify "$BASE_REF" >/dev/null 2>&1; then
  echo "doc-validate: no base ref; skipping (first commit or shallow clone)."
  exit 0
fi

MODE="${DOC_VALIDATE_MODE:-}"
if [[ -z "$MODE" ]] && command -v jq >/dev/null 2>&1; then
  MODE="$(jq -r '.mode // "warn"' "$CONFIG")"
fi
MODE="${MODE:-warn}"

echo "doc-validate: comparing $BASE_REF..HEAD (mode=$MODE)"

mapfile -t CHANGED < <(git diff --name-only "$BASE_REF"...HEAD 2>/dev/null || git diff --name-only "$BASE_REF" HEAD)

if [[ ${#CHANGED[@]} -eq 0 ]]; then
  echo "doc-validate: no changed files."
  exit 0
fi

only_docs=true
for f in "${CHANGED[@]}"; do
  case "$f" in
    docs/*|*.md|README.md|DEVELOPER_GUIDE.md|FrontEnd/FRONTEND_STARTER.md|FrontEnd/COREUI_INTEGRATION.md|BackEnd/DOCKER_ENVIRONMENT.md)
      ;;
    *)
      only_docs=false
      break
      ;;
  esac
done

if $only_docs; then
  echo "doc-validate: documentation-only changes — OK."
  exit 0
fi

violations=()

path_changed() {
  local prefix="$1"
  for f in "${CHANGED[@]}"; do
    [[ "$f" == "$prefix"* ]] && return 0
  done
  return 1
}

any_doc_changed() {
  for prefix in "$@"; do
    for f in "${CHANGED[@]}"; do
      [[ "$f" == "$prefix"* ]] && return 0
    done
  done
  return 1
}

check_mapping() {
  local name="$1"
  shift
  local -a code_prefixes=()
  local -a doc_prefixes=()
  local in_code=true
  for arg in "$@"; do
    if [[ "$arg" == "--" ]]; then in_code=false; continue; fi
    if $in_code; then code_prefixes+=("$arg"); else doc_prefixes+=("$arg"); fi
  done

  local code_hit=false
  for p in "${code_prefixes[@]}"; do
    if path_changed "$p"; then code_hit=true; break; fi
  done
  [[ "$code_hit" == false ]] && return 0

  if any_doc_changed "${doc_prefixes[@]}"; then
    return 0
  fi

  violations+=("$name: code under ${code_prefixes[*]} changed but no docs in ${doc_prefixes[*]}")
}

# Parse mappings from JSON without jq (fallback) or with jq
if command -v jq >/dev/null 2>&1; then
  count="$(jq '.mappings | length' "$CONFIG")"
  for ((i=0; i<count; i++)); do
    name="$(jq -r ".mappings[$i].name" "$CONFIG")"
    mapfile -t codes < <(jq -r ".mappings[$i].codePaths[]" "$CONFIG")
    mapfile -t docs < <(jq -r ".mappings[$i].docPaths[]" "$CONFIG")
    local code_hit=false
    for p in "${codes[@]}"; do
      if path_changed "$p"; then code_hit=true; break; fi
    done
    if $code_hit && ! any_doc_changed "${docs[@]}"; then
      violations+=("$name: code changed; expected doc update under: ${docs[*]}")
    fi
  done
else
  # Minimal hardcoded fallback when jq missing
  check_mapping auth BackEnd/src/Modules/Auth/ -- docs/modules/auth/ docs/api/auth.md
  check_mapping tenant BackEnd/src/Modules/Tenant/ -- docs/modules/tenant/ docs/api/tenant.md
  check_mapping users BackEnd/src/Modules/Users/ -- docs/modules/users/ docs/api/users.md
  check_mapping audit BackEnd/src/Modules/Audit/ -- docs/modules/audit/ docs/api/audit.md
  check_mapping caching BackEnd/src/Modules/Caching/ -- docs/modules/caching/
  check_mapping host BackEnd/src/Host/ -- docs/modules/host/ docs/architecture/ docs/operations/
  check_mapping frontend FrontEnd/apps/web/src/ -- docs/frontend/
fi

if [[ ${#violations[@]} -gt 0 ]]; then
  echo ""
  echo "=== Documentation validation ($MODE) ==="
  for v in "${violations[@]}"; do
    echo "  - $v"
  done
  echo ""
  echo "See docs/documentation-pr-checklist.md and docs/developer-workflow.md"
  if [[ "$MODE" == "fail" ]]; then
    exit 1
  fi
  exit 0
fi

echo "doc-validate: all mapped code changes have corresponding doc updates."
exit 0
